using ARAKDataSetup.infrastructure.Contracts.IDataAccessor;

namespace ARAKDataSetup.infrastructure.Contracts.IFactories
{
    public interface IDataAccessFactory
    {
        IDataAccess ServerDataAccess();
    }
}