using Microsoft.Data.SqlClient;

namespace BallastLane.TaskManager.Infrastructure.Data;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}
