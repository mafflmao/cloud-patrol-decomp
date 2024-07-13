public interface ILinkable
{
	string ToyLinkDisplayName { get; }

	string ToyLinkCardInstructionText { get; }

	bool MatchesToyAndSubtype(uint linkedToyId, uint linkedToySubtypeId);

	void UnlockFromToy(uint linkToySubtypeId);
}
