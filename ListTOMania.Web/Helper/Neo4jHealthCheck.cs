using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ListTOMania.Web.Helper
{
    public class Neo4jHealthCheck : IHealthCheck
    {
        private readonly IGraphClient _graphClient;

        public Neo4jHealthCheck(IGraphClient graphClient)
        {
            _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Replace this with your actual Neo4j query to check the database connection.
                // For example, you can run a simple query to verify the database's availability.
                var test = await _graphClient.Cypher.Match("(n").Limit(1).Return<int>("count(n)").ResultsAsync;

                if (test.Single() == 1)
                {
                    return HealthCheckResult.Healthy("Neo4j database is available.");
                }
                else
                {
                    return HealthCheckResult.Unhealthy("Neo4j database is not responding as expected or database is empty (Create at least the admin user!).");
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("An error occurred while checking the Neo4j database.", ex);
            }
        }
    }
}