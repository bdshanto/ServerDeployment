using ServerDeployment.Domains.Utility;
using ServerDeployment.infrastructure.Contracts.IDataAccessor;
using ServerDeployment.infrastructure.Contracts.IFactories;
using ServerDeployment.infrastructure.DataAccessor;

namespace ServerDeployment.infrastructure.Factories
{
    public class DataAccessFactory : IDataAccessFactory
    {
        public IDataAccess ServerDataAccess()
        {
            IDataAccess dataAccess = new DataAccess(AppUtility.ConnectionString);
            return dataAccess;
        }

    }
}