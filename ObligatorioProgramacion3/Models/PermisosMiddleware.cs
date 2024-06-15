public class PermissionMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
  ;
    }
}

public static class PermissionMiddlewareExtensions
{
    public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PermissionMiddleware>();
    }
}