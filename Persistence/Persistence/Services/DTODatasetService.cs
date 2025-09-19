using MongoDB.Bson;
using MongoDB.Driver;
using Persistence.Interfaces;
using Persistence.Models;

namespace Persistence.Services
{
    public class DTODatasetService
    {
        private readonly IMongoCollection<DTODataset> _dtoDatasets;

        public DTODatasetService(IDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            _dtoDatasets = database.GetCollection<DTODataset>(settings.CollectionName);
        }

        public async ValueTask<List<DTODataset>> GetAll()
        {
            return await _dtoDatasets.Find<DTODataset>((DTODataset set) => true).ToListAsync();
        }

        public async ValueTask<DTODataset> GetByMetadata(List<string> symbols, DateTime dateFrom, DateTime dateTo)
        {
            return await _dtoDatasets.Find<DTODataset>((DTODataset row) => row.Metadata.DateTo == dateTo && row.Metadata.DateFrom == dateFrom && row.Metadata.Symbols.All((string s) => symbols.Contains(s))).FirstOrDefaultAsync();
        }

        public async ValueTask<DTODataset> GetById(ObjectId id, CancellationToken ct)
        {
            return await IMongoCollectionExtensions.Find<DTODataset>(filter: Builders<DTODataset>.Filter.Eq((DTODataset row) => row.Id, id), collection: _dtoDatasets).FirstOrDefaultAsync();
        }

        public async ValueTask<DTODataset> Add(DTODataset toAdd, CancellationToken cancellationToken)
        {
            Task insertionTask = _dtoDatasets.InsertOneAsync(toAdd, null, cancellationToken);
            await insertionTask;
            if (insertionTask.IsCompletedSuccessfully)
            {
                return await GetByMetadata(toAdd.Metadata.Symbols, toAdd.Metadata.DateFrom, toAdd.Metadata.DateTo);
            }
            return null;
        }

        public async ValueTask<DTODataset> Update(ObjectId id, DTODataset toUpdate, CancellationToken cancellationToken)
        {
            FilterDefinition<DTODataset> filter = Builders<DTODataset>.Filter.Eq((DTODataset row) => row.Id, id);
            UpdateDefinition<DTODataset> update = Builders<DTODataset>.Update.Set("Metadata", toUpdate.Metadata).Set("EODData", toUpdate.EODData);
            Task updateTask = _dtoDatasets.UpdateOneAsync(filter, update, null, cancellationToken);
            await updateTask;
            if (updateTask.IsCompletedSuccessfully)
            {
                return await GetByMetadata(toUpdate.Metadata.Symbols, toUpdate.Metadata.DateFrom, toUpdate.Metadata.DateTo);
            }
            return null;
        }

        public async Task<bool> DeleteById(ObjectId toDelete, CancellationToken ct)
        {
            Task<DeleteResult> output = _dtoDatasets.DeleteOneAsync<DTODataset>((DTODataset ds) => ds.Id == toDelete);
            await output;
            return output.IsCompletedSuccessfully;
        }
    }
}
