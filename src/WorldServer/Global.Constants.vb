'Global.Constants.vb
'
'Rift .NET Reloaded -- An OpenSource Server Emulator for World of Warcraft Classic Alpha 0.5.3 (3368) written in VB.Net
'Copyright (c) 2012 noVo aka. takeoYasha

'This program is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
