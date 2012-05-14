Public Module Global_Constants

    Class GlobalConstants

#Region "Account.Enums"

        Public Enum AccessLevel As Byte
            Trial = 0
            Player = 1
            GameMaster = 2
            Admin = 3
            Developer = 4
        End Enum

#End Region


#Region "Player.Enums"

        Public Enum Genders As Byte
            GENDER_MALE = 0
            GENDER_FEMALE = 1
        End Enum

        Public Enum Classes As Byte
            CLASS_WARRIOR = 1
            CLASS_PALADIN = 2
            CLASS_HUNTER = 3
            CLASS_ROGUE = 4
            CLASS_PRIEST = 5
            CLASS_SHAMAN = 7
            CLASS_MAGE = 8
            CLASS_WARLOCK = 9
            CLASS_DRUID = 11
        End Enum

        Public Enum Races As Byte
            RACE_HUMAN = 1
            RACE_ORC = 2
            RACE_DWARF = 3
            RACE_NIGHT_ELF = 4
            RACE_UNDEAD = 5
            RACE_TAUREN = 6
            RACE_GNOME = 7
            RACE_TROLL = 8
        End Enum

#End Region

    End Class

End Module
