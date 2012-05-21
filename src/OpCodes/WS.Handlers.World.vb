'WS.Handlers.World.vb
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

Imports System.Data

Public Class WS_Handlers_World

    Public Shared Function Build_SMSG_UPDATE_OBJECT(ByRef packet As PacketReader, ByRef response As PacketWriter, ByRef Client As WorldServerClass) As PacketWriter
        Dim PlayerGUID As UInt64 = packet.ReadUInt64()

        'Get Char by GUID from Database ...
        Client.Character.GetCharacterByGuid(PlayerGUID)

        response.WriteUInt32(1) ' ObjectCount
        response.WriteUInt8(2)  ' UpdateType, UPDATE_FULL/CreateObject) + PartialUpdateFromFullUpdate
        response.WriteUInt64(Client.Character.GUID) ' ObjectGuid
        response.WriteUInt8(4)  ' ObjectType, 4 = Player
        response.WriteUInt64(0) ' TransportGuid
        response.WriteSingle(0) ' TransportX
        response.WriteSingle(0) ' TransportY
        response.WriteSingle(0) ' TransportZ
        response.WriteSingle(0) ' TransportW (TransportO)

        response.WriteSingle(Client.Character.PositionX) ' x
        response.WriteSingle(Client.Character.PositionY) ' y
        response.WriteSingle(Client.Character.PositionZ) ' z
        response.WriteSingle(Client.Character.Facing)    ' w/o (facing)
        response.WriteSingle(0) ' Pitch
        response.WriteUInt32(&H8000000) ' MovementFlagMask
        response.WriteUInt32(0)       ' FallTime
        response.WriteSingle(2.5)    ' WalkSpeed
        response.WriteSingle(7.0)    ' RunSpeed
        response.WriteSingle(4.7222) ' SwimSpeed
        response.WriteSingle(3.14)   ' TurnSpeed
        response.WriteUInt32(1)       ' Flags, 1 - Player
        response.WriteUInt32(1)       ' AttackCycle
        response.WriteUInt32(0)       ' TimerId
        response.WriteUInt64(0)       ' VictimGuid

        ' FillInPartialObjectData
        response.WriteUInt8(&H14)
        ' UpdateMaskBlocks, 20
        For i As Integer = 0 To 19
            response.WriteUInt32(&HFFFFFFFFUI)
        Next

        ' ObjectFields
        response.WriteUInt64(Client.Character.GUID)
        response.WriteUInt32(&H19)
        ' UpdateType, 0x19 - Player (Player + Unit + Object)
        response.WriteUInt32(0)
        response.WriteSingle(1)
        response.WriteUInt32(0)

        ' UnitFields
        For i As Integer = 0 To 15
            response.WriteUInt32(0)
        Next

        response.WriteUInt32(Client.Character.Health)
        response.WriteUInt32(Client.Character.Mana)
        response.WriteUInt32(Client.Character.Rage)
        response.WriteUInt32(Client.Character.Focus)
        response.WriteUInt32(Client.Character.Energy)

        ' Max values...
        response.WriteUInt32(Client.Character.Health)
        response.WriteUInt32(Client.Character.Mana)
        response.WriteUInt32(Client.Character.Rage)
        response.WriteUInt32(Client.Character.Focus)
        response.WriteUInt32(Client.Character.Energy)
        response.WriteUInt32(Client.Character.Level)
        response.WriteUInt32(5)
        response.WriteUInt8(Client.Character.Race)
        response.WriteUInt8(Client.Character.Classe)
        response.WriteUInt8(Client.Character.Gender)
        response.WriteUInt8(1)

        response.WriteUInt32(Client.Character.Strength)
        response.WriteUInt32(Client.Character.Agility)
        response.WriteUInt32(Client.Character.Stamina)
        response.WriteUInt32(Client.Character.Intellect)
        response.WriteUInt32(Client.Character.Spirit)

        ' BastStats, copy ...
        response.WriteUInt32(Client.Character.Strength)
        response.WriteUInt32(Client.Character.Agility)
        response.WriteUInt32(Client.Character.Stamina)
        response.WriteUInt32(Client.Character.Intellect)
        response.WriteUInt32(Client.Character.Spirit)

        For i As Integer = 0 To 9
            response.WriteUInt32(0)
        Next

        ' Money
        response.WriteUInt32(Client.Character.Money)

        For i As Integer = 0 To 55
            response.WriteUInt32(0)
        Next

        For i As Integer = 0 To 38
            response.WriteUInt32(0)
        Next

        ' DisplayId
        Select Case Client.Character.Race
            Case GlobalConstants.Races.RACE_HUMAN
                response.WriteUInt32(CUInt(If(Client.Character.Gender = 0, &H31, &H32)))
                Exit Select
            Case GlobalConstants.Races.RACE_ORC
                response.WriteUInt32(CUInt(If(Client.Character.Gender = 0, &H33, &H34)))
                Exit Select
            Case GlobalConstants.Races.RACE_DWARF
                response.WriteUInt32(CUInt(If(Client.Character.Gender = 0, &H35, &H36)))
                Exit Select
            Case GlobalConstants.Races.RACE_NIGHT_ELF
                response.WriteUInt32(CUInt(If(Client.Character.Gender = 0, &H37, &H38)))
                Exit Select
            Case GlobalConstants.Races.RACE_UNDEAD
                response.WriteUInt32(CUInt(If(Client.Character.Gender = 0, &H39, &H40)))
                Exit Select
            Case GlobalConstants.Races.RACE_TAUREN
                response.WriteUInt32(CUInt(If(Client.Character.Gender = 0, &H3B, &H3C)))
                Exit Select
            Case GlobalConstants.Races.RACE_GNOME
                response.WriteUInt32(CUInt(If(Client.Character.Gender = 0, &H61B, &H61C)))
                Exit Select
            Case GlobalConstants.Races.RACE_TROLL
                response.WriteUInt32(CUInt(If(Client.Character.Gender = 0, &H5C6, &H5C7)))
                Exit Select
        End Select

        For i As Integer = 0 To 31
            response.WriteUInt32(0)
        Next

        ' PlayerFields
        For i As Integer = 0 To 45
            response.WriteUInt32(0)
        Next

        For i As Integer = 0 To 31
            response.WriteUInt32(0)
        Next

        For i As Integer = 0 To 47
            response.WriteUInt32(0)
        Next

        For i As Integer = 0 To 11
            response.WriteUInt32(0)
        Next

        response.WriteUInt32(0)
        response.WriteUInt32(0)

        response.WriteUInt32(0)
        response.WriteUInt32(0)

        response.WriteUInt32(0)
        response.WriteUInt32(0)

        ' InventarSlots
        response.WriteUInt32(14)

        response.WriteUInt32(0)
        response.WriteUInt32(0)

        ' PLAYER_BYTES (Skin, Face, HairStyle, HairColor)
        response.WriteUInt8(Client.Character.Skin)
        response.WriteUInt8(Client.Character.Face)
        response.WriteUInt8(Client.Character.HairStyle)
        response.WriteUInt8(Client.Character.HairColor)

        ' XP
        response.WriteUInt32(0)

        ' NextLevel
        response.WriteUInt32(1)

        ' SkillInfo
        For i As Integer = 0 To 191
            response.WriteUInt32(0)
        Next

        ' PLAYER_BYTES_2 (FacialHair, Unknown, BankBagSlotCount, RestState)
        response.WriteUInt8(Client.Character.FacialHair)
        response.WriteUInt8(0)
        response.WriteUInt8(0)
        response.WriteUInt8(1)

        ' QuestInfo
        For i As Integer = 0 To 95
            response.WriteUInt32(0)
        Next

        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)
        response.WriteUInt32(0)

        ' BaseMana
        response.WriteUInt32(20)
        response.WriteUInt32(0)

        ' xD?
        response.WriteUInt8(0)
        response.WriteUInt8(0)
        response.WriteUInt8(0)


        Return response
    End Function

End Class