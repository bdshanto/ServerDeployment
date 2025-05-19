using ARAKDataSetup.Domains.Utility;
using ARAKDataSetup.infrastructure.Contracts.IDataAccessor;
using ARAKDataSetup.infrastructure.Contracts.IFactories;
using ARAKDataSetup.infrastructure.DataAccessor;

namespace ARAKDataSetup.infrastructure.Factories
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