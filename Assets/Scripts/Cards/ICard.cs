using Cards;

public interface ICard {
	Card.Suit TypeOfSuit { get; }
	int Value { get;}

	void EnableCard();
	void DisableCard();
}
