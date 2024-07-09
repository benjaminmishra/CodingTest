public static class Endpoints
{
    public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("/", Handler);
        return endpointRouteBuilder;
    }

    public static string Handler()
    {
        return "hello worldsssssss";
    }
}