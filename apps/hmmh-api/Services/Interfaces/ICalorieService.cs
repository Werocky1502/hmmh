using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Db.Models;

namespace Hmmh.Api.Services;

/// <summary>
///     Defines calorie tracking workflows for the API.
/// </summary>
public interface ICalorieService
{
    /// <summary>
    ///     Retrieves calorie entries for a specific date.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="date">Entry date.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>List of calorie responses for the date.</returns>
    Task<IReadOnlyList<CalorieEntryResponse>> GetCaloriesByDateAsync(
        Guid userId,
        DateOnly date,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Retrieves calorie entries for a date range.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="startDate">Start of the range (inclusive).</param>
    /// <param name="endDate">End of the range (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>List of calorie responses for the range.</returns>
    Task<IReadOnlyList<CalorieEntryResponse>> GetCaloriesAsync(
        Guid userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Creates a new calorie entry.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="request">Calorie entry request payload.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Saved calorie entry response.</returns>
    Task<CalorieEntryResponse> CreateCalorieAsync(
        Guid userId,
        CalorieEntryRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a calorie entry.
    /// </summary>
    /// <param name="userId">User identifier.</param>
    /// <param name="entryId">Entry identifier.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>Task representing the operation.</returns>
    Task DeleteCalorieAsync(Guid userId, Guid entryId, CancellationToken cancellationToken);
}
