using System;

namespace Bidvalet.Model
{
    public enum Legs
    {
        ZeroLeg = 0,
        OneLeg,
        TwoLegs,
        ThreeLegs,
        FourLegs,
        FiveLegs,
        SixLegs,
        SevenLegs,
        EightLegs,
        NineLegs
    }
    public enum Dow
    {
        Mon = 0,
        Tue,
        Wed,
        Thu,
        Fri,
        Sat,
        Sun
    }

    public enum ConstraintType
    {
        LessThan = 1,
        EqualTo,
        MoreThan,
        NotEqualTo,
        atafter,
        atbefore
    }
    public enum AMPMType
    {
        AM = 1,
        PM,
        MIX,
        NTE
    }
    public enum WeightType
    {
        Less,
        Equal,
        More,
        NotEqual,
    }
    public enum DeadheadType
    {
        First = 1,
        Last,
        Both,
        Either
    }
    public enum FAPositon
    {
        A = 1,
        B,
        C,
        D
    }

    public enum RestType
    {
        All = 1,
        InDomicile,
        AwayDomicile
    }

    public enum CityType
    {
        International = 1,
        NonConus
    }

    public enum RestOptions
    {
        Shorter = 1,
        Longer,
        Both
    }

    public enum DutyPeriodType
    {
        Relative = 1,
        Longer,
        Shorter
    }

    public enum LegsPerPairingType
    {
        Less = 1,
        More,
        All
    }

    public enum ThreeOnThreeOff
    {
        ThreeOnThreeOff = 1,
        NoThreeOnThreeOff,

    }
    public enum BidLineType
    {
        NormalTrip = 0,
        NoTrip,
        VA,
        VAP,
        VO,
        VD,
        VOFSplit,
        VOBSplit,
        VDDrop,
        VOFSplitDrop,
        VOBSplitDrop,
        FV

    }
    public enum ModernBidlineBorderType
    {
        Noborder,
        singleLineBorder
    }
    public enum CalenderColortype
    {
        VA = 0,
        VO,
        VD,
        VOB,
        VOF,
        Transparaent,
        MILVO_No_Work,
        MILBackSplitWork,
        MILFrontSplitWork,
        MILBackSplitWithoutStrike,
        MILFrontSplitWithoutStrike,
        FV
    }

    public enum DayofTheWeek
    {
        Monday = 0,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public enum DutyPeriodIcon
    {
        RoundedLeft = 1,
        SquareCorner,
        RoundedRight,
        RoundedOnBoth
    }
    public enum BidFileType
    {
        NormalLine = 1,
        Trip,
        Vacation,
        VacationDropOFF,
        Eom,
        EomDropOFF,
        VacationEOM,
        VacationEomDropOFF,

    }
}

