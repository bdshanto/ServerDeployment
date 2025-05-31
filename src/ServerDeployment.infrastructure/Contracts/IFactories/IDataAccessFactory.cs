using ServerDeployment.infrastructure.Contracts.IDataAccessor;

namespace ServerDeployment.infrastructure.Contracts.IFactories
{
    public interface IDataAccessFactory
    {
        IDataAccess ServerDataAccess();
    }
}