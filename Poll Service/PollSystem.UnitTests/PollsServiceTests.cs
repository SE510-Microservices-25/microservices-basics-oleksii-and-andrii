using MassTransit;
using Moq;
using PollSystem.Entities;
using PollSystem.Repositories;
using PollSystem.Services;

namespace PollSystem.UnitTests;

[TestClass]
public class PollsServiceTests
{
	private Mock<IPollsRepository> _repositoryMock = null!;
	private IBus _busMock = null!;
	private PollsService _service;

	[TestInitialize]
	public void TestInitialize()
	{
		_repositoryMock = new Mock<IPollsRepository>();
		_busMock = Mock.Of<IBus>();
		_service = new PollsService(_repositoryMock.Object, _busMock);
	}

	[TestMethod]
	public async Task GetAllPollsAsync_ShouldReturnAllPolls()
	{
		// Arrange
		List<Poll> pollsToReturn = DataGenerator.GeneratePolls();
		_repositoryMock
			.Setup(repository => repository.GetAllPollsAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(pollsToReturn);

		// Act
		List<Poll> polls = await _service.GetAllPollsAsync(CancellationToken.None);

		// Assert
		CollectionAssert.AreEqual(polls, pollsToReturn);
	}

	[TestMethod]
	public async Task GetPollByIdAsync_ShouldReturnPoll_WhenPollWithIdExists()
	{
		// Arrange
		Poll pollToReturn = DataGenerator.GeneratePolls(1)[0];
		int id = 1;
		_repositoryMock
			.Setup(repository => repository.GetPollByIdAsync(id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(pollToReturn);

		// Act
		Poll? poll = await _service.GetPollByIdAsync(id, CancellationToken.None);

		// Assert
		Assert.AreEqual(pollToReturn, poll);
	}

	[TestMethod]
	public async Task GetPollByIdAsync_ShouldReturnNull_WhenPollWithIdDoesNotExists()
	{
		// Arrange
		int id = 1;

		// Act
		Poll? poll = await _service.GetPollByIdAsync(id, CancellationToken.None);

		// Assert
		Assert.IsNull(poll);
	}

	[TestMethod]
	public async Task CreatePollAsync_ShouldReturnCreatedPoll()
	{
		// Arrange
		Poll newPoll = DataGenerator.GeneratePolls(1)[0];
		PollCreateDto pollCreateDto = new PollCreateDto()
		{
			ExpirationDate = newPoll.ExpirationDate,
			Question = newPoll.Question,
			Options = newPoll.Options
				.Select(option => new PollOptionCreateDto() { Text = option.Text })
				.ToList()
		};

		_repositoryMock
			.Setup(
				repository => repository.CreatePollAsync(
					It.IsAny<Poll>(),
					It.IsAny<CancellationToken>()
				)
			)
			.ReturnsAsync(newPoll);

		_repositoryMock
			.Setup(
				repository => repository.AddOptionsAsync(
					It.IsAny<List<PollOption>>(),
					It.IsAny<CancellationToken>()
				)
			)
			.ReturnsAsync(true);

		// Act
		Poll? poll = await _service.CreatePollAsync(pollCreateDto, CancellationToken.None);

		// Assert
		Assert.IsNotNull(poll);
		Assert.AreEqual(poll.Id, newPoll.Id);
		Assert.AreEqual(poll.Question, newPoll.Question);
		CollectionAssert.AreEqual(poll.Options.ToList(), newPoll.Options.ToList());
	}

	[TestMethod]
	public async Task UpdatePollAsync_ShouldReturnUpdatedPoll()
	{
		// Arrange
		Poll pollToUpdate = DataGenerator.GeneratePolls(1)[0];
		PollUpdateDto pollUpdateDto = new PollUpdateDto()
		{
			Question = "Fortnite or PUBG?",
			ExpirationDate = pollToUpdate.ExpirationDate.AddDays(2),
			Options = new List<PollOptionCreateDto>()
		};

		Poll pollToExpect = pollToUpdate;
		pollToExpect.Question = pollUpdateDto.Question;
		pollToExpect.ExpirationDate = pollUpdateDto.ExpirationDate;
		pollToExpect.Options = new List<PollOption>();

		_repositoryMock
			.Setup(
				repository => repository.UpdatePollAsync(
					It.IsAny<int>(),
					It.IsAny<PollUpdateDto>(),
					It.IsAny<CancellationToken>()
				)
			)
			.ReturnsAsync(pollToUpdate);

		// Act
		await _service.UpdatePollAsync(pollToUpdate.Id, pollUpdateDto, CancellationToken.None);

		// Assert
		_repositoryMock.Verify(
			repository => repository.UpdatePollAsync(
				pollToUpdate.Id,
				pollUpdateDto,
				It.IsAny<CancellationToken>()
			),
			Times.Once
		);
	}

	[TestMethod]
	public async Task DeletePollAsync_ShouldNotThrowException()
	{
		// Arrange
		_repositoryMock
			.Setup(
				repository => repository.DeletePollAsync(
					It.IsAny<int>(),
					It.IsAny<CancellationToken>()
				)
			)
			.ReturnsAsync(true);

		// Act
		await _service.DeletePollAsync(1, CancellationToken.None);

		// Assert
		_repositoryMock.Verify(
			repository => repository.DeletePollAsync(
				It.IsAny<int>(),
				It.IsAny<CancellationToken>()
			),
			Times.Once
		);
	}
}
