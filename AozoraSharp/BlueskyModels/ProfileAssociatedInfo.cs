using AozoraSharp.HttpObjects;

namespace AozoraSharp.BlueskyModels;

public readonly struct ProfileAssociatedInfo(int numListsCreated, int numFeedsCreated, bool isLabeler)
{
    public int NumListsCreated { get; } = numListsCreated;
    public int NumFeedsCreated { get; } = numFeedsCreated;
    public bool IsLabeler { get; } = isLabeler;

    internal ProfileAssociatedInfo(ProfileAssociated profileAssociated) : this(profileAssociated.Lists, profileAssociated.Feedgens, profileAssociated.Labeler) { }
}
