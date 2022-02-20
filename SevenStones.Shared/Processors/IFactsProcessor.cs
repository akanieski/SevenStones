using SevenStones.Models;

namespace SevenStones.Processors
{
    public abstract class BaseFactsProcessor
    {
        protected void UpsertFact(RepositoryBranch repoBranch, FactType factType, string file, string value, string location = "", string actionUrl = "")
        {
            var existingFact = repoBranch.Facts.FirstOrDefault(f => f.Path == file && f.FactType == factType);
            if (existingFact == null)
            {
                repoBranch.Facts.Add(existingFact = new Fact()
                {
                    FactType = factType,
                    Path = file,
                    RepositoryBranchId = repoBranch.Id,
                    RepositoryBranch = repoBranch,
                    ActionLink = actionUrl
                });
            }
            existingFact.Value = value;
            existingFact.ActionLink = actionUrl;
            existingFact.Location = location;
        }
    }
}
