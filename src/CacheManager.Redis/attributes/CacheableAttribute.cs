using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using CacheManager.Redis.Enums;
using CacheManager.Redis.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CacheManager.Redis.Attributes;

public class CacheableAttribute : TypeFilterAttribute
{
    public CacheableAttribute(Type entityType) : 
        base(typeof(CacheableAttribute<>).MakeGenericType(entityType))
    {
    }   
    
    public CacheableAttribute(Type entityType, CacheableKeyType keyType, string key) : 
        base(typeof(CacheableAttribute<>).MakeGenericType(entityType))
    {
        var objects = new List<object>
        {
            key,
            keyType
        };

        Arguments = objects.ToArray();
    }   
}

public class CacheableAttribute<TEntity> : IAsyncActionFilter where TEntity : class
{
    private readonly IRedisCacheManager<TEntity> _cacheManager;
    private string _key = string.Empty;
    private CacheableKeyType _keyType = CacheableKeyType.MethodName;

    public CacheableAttribute(IRedisCacheManager<TEntity> cacheManager)
        => _cacheManager = cacheManager;
    
    public CacheableAttribute(IRedisCacheManager<TEntity> cacheManager, string key, CacheableKeyType keyType)
    {
        _cacheManager = cacheManager;
        _key = key;
        _keyType = keyType;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Guard.Against.Null(context);
        Guard.Against.Null(next);

        ProduceKey(context);
        OnActionExecuting(context);
        if (context.Result == null)
        {
            OnActionExecuted(await next());
        }
    }
            
    public virtual void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid) return;
                
        if (_cacheManager.TryGet(_key, out var cachedBook) && cachedBook is not null)
            context.Result = new OkObjectResult(cachedBook);
    }
            
    public virtual void OnActionExecuted(ActionExecutedContext context)
    {
        if (context is not { Canceled: false, HttpContext.Response.StatusCode: 200, Result: not null }) return;
        
        var result = ((JsonResult)context.Result).Value;
        if (result is TEntity entity)
            _cacheManager.Set(_key!, entity);
    }

    private void ProduceKey(ActionExecutingContext context)
    {
        switch (_keyType)
        {
            case CacheableKeyType.FromRouteOrQuery:
                _key = context.ActionArguments[_key]?.ToString()??"";
                break;
            case CacheableKeyType.MethodName:
                _key = context.ActionDescriptor.DisplayName ?? "";
                break;
            case CacheableKeyType.FromModel:
                var modelNameSplit = _key.Split(".");
                if (modelNameSplit.Length != 2)
                    break;
                var modelName = modelNameSplit[0];
                var model = context.ActionArguments[modelName];
                if (model is not null)
                {
                    var modelType = model.GetType();
                    var keyProperty = modelType.GetProperty(modelNameSplit[1]);
                    if (keyProperty is null)
                        break;
                    
                    _key = keyProperty.GetValue(model)?.ToString()??"";
                        break;
                }
                _key = "";
                break;
            case CacheableKeyType.FromProvidedValue:
            default:
                break;
        }

        if (string.IsNullOrWhiteSpace(_key))
            _key = Guard.Against.Null(context.ActionDescriptor.DisplayName);
    }
}