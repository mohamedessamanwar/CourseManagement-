using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.Exceptions;


namespace Restaurant.Api.Middlewares
{
    public class ErrorHandleMiddleware : IMiddleware

    {

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {

                await next.Invoke(context);
            }
            catch (NotFountException ex)
            {
                var res = new Response<string>(ex.Message, false, "faild");

                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json"; // Set the content type to JSON

                // Serialize the response object to JSON and write it to the response
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(res));

            }
            catch (NotModifiedException ex)
            {
                var res = new Response<string>(ex.Message, false, "faild");

                context.Response.StatusCode = 304;
                context.Response.ContentType = "application/json"; // Set the content type to JSON

                // Serialize the response object to JSON and write it to the response
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(res));

            }
            catch (Exception ex) { }

        }





    }
}
