using Microsoft.EntityFrameworkCore;
using PollSystem.Data;
using PollSystem.Entities;
using PollSystem.Repositories;

namespace PollSystem.UnitTests;

[TestClass]
public class PollsRepositoryTests
{
	private AppDbContext _context;
	private PollsRepository _repository;

	[TestInitialize]
	public void TestInitialize()
	{
		DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		_context = new AppDbContext(options);
		_repository = new PollsRepository(_context);
	}

	[TestCleanup]
	public void TestCleanup()
	{
		_context.Database.EnsureDeleted();
		_context.Dispose();
	}

	[TestMethod]
	public async Task GetAllAsync_ShouldReturnAllPolls_WhenDbSetIsNotEmpty()
	{
		// Arrange
		List<Poll> items = DataGenerator.GeneratePolls();

		await _context.Polls.AddRangeAsync(items);
		await _context.SaveChangesAsync();

		// Act
		List<Poll> result = (await _repository.GetAllPollsAsync(CancellationToken.None)).ToList();

		// Assert
		CollectionAssert.AreEqual(items, result);
	}

	[TestMethod]
	public async Task GetAllAsync_ShouldReturnAllPolls_WhenDbSetIsEmpty()
	{
		// Act
		List<Poll> result = (await _repository.GetAllPollsAsync(CancellationToken.None)).ToList();

		// Assert
		CollectionAssert.AreEqual(new List<Poll>(), result);
	}

	[TestMethod]
	public async Task GetByIdAsync_ShouldReturnSinglePoll_WhenExists()
	{
		// Arrange
		Poll item = DataGenerator.GeneratePolls(1)[0];

		await _context.Polls.AddAsync(item);
		await _context.SaveChangesAsync();

		// Act
		Poll? result = await _repository.GetPollByIdAsync(item.Id, CancellationToken.None);

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual(item, result);
	}

	[TestMethod]
	public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
	{
		// Act
		Poll? result = await _repository.GetPollByIdAsync(404, CancellationToken.None);

		// Assert
		Assert.IsNull(result);
	}

	[TestMethod]
	public async Task CreatePollAsync_ShouldReturnCreatedPoll()
	{
		// Arrange
		Poll item = DataGenerator.GeneratePolls(1)[0];

		// Act
		await _repository.CreatePollAsync(item, CancellationToken.None);

		// Assert
		Assert.AreEqual(1, await _context.Polls.CountAsync());
	}

	[TestMethod]
	public async Task AddOptionsAsync_ShouldReturnTrue_OnSuccess()
	{
		// Arrange
		List<PollOption> items = DataGenerator.GeneratePollOptions();

		// Act
		bool result = await _repository.AddOptionsAsync(items, CancellationToken.None);

		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(items.Count, await _context.PollOptions.CountAsync());
	}

	[TestMethod]
	public async Task UpdatePollAsync_ShouldReturnUpdatedPoll()
	{
		// Arrange
		Poll poll = DataGenerator.GeneratePolls(1)[0];

		await _context.Polls.AddAsync(poll);
		await _context.SaveChangesAsync();

		// Act
		string newQuestion = "Fortnite or PUBG?";
		DateTime newExpirationDate = DateTime.UtcNow.AddDays(24);

		await _repository.UpdatePollAsync(
			poll.Id,
			new PollUpdateDto()
			{
				Question = newQuestion,
				ExpirationDate = newExpirationDate
			},
			CancellationToken.None
		);

		// Assert
		Poll? updatedPoll = await _repository.GetPollByIdAsync(poll.Id, CancellationToken.None);
		Assert.IsNotNull(updatedPoll);
		Assert.AreEqual(updatedPoll.Question, newQuestion);
		Assert.AreEqual(updatedPoll.ExpirationDate, newExpirationDate);
		CollectionAssert.AreEqual(updatedPoll.Options.ToList(), new List<PollOption>());
	}

	[TestMethod]
	public async Task DeletePollAsync_ShouldReturnTrue_OnSuccess()
	{
		// Arrange
		Poll poll = DataGenerator.GeneratePolls(1)[0];

		await _context.Polls.AddAsync(poll);
		await _context.SaveChangesAsync();

		// Act
		bool result = await _repository.DeletePollAsync(poll.Id, CancellationToken.None);

		// Assert
		Assert.IsTrue(result);
		Assert.AreEqual(0, await _context.Polls.CountAsync());
		Assert.AreEqual(0, await _context.PollOptions.CountAsync());
	}

	[TestMethod]
	public async Task DeletePollAsync_ShouldReturnFalse_OnSuccess()
	{
		// No arrange - no data

		// Act
		bool result = await _repository.DeletePollAsync(404, CancellationToken.None);

		// Assert
		Assert.IsFalse(result);
	}
}
