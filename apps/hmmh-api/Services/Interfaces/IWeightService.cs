using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Db.Models;

namespace Hmmh.Api.Services;

/// <summary>
///     Defines weight tracking workflows for the API.
/// </summary>
public interface IWeightService
{
    /// <summary>
    ///     Retrieves the weight entry for a specific date.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="date">Entry date.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Weight response for the supplied date.</returns>
    Task<WeightEntryResponse> GetWeightByDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves weight entries for a date range.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="startDate">Start of the range (inclusive).</param>
    /// <param name="endDate">End of the range (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>List of weight responses in the range.</returns>
    Task<IReadOnlyList<WeightEntryResponse>> GetWeightsAsync(
        Guid userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Creates or updates a weight entry.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="request">Weight entry request payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Saved weight entry response.</returns>
    Task<WeightEntryResponse> UpsertWeightAsync(
        Guid userId,
        WeightEntryRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a weight entry.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="entryId">Entry identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Task representing the operation.</returns>
    Task DeleteWeightAsync(Guid userId, Guid entryId, CancellationToken cancellationToken);
}
