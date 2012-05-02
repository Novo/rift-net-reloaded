﻿Imports System.IO
Imports System.Text

Public Module Packets

    Public Class PacketReader
        Inherits BinaryReader

        Public Property Opcode() As OpCodes
            Get
                Return m_Opcode
            End Get
            Set(ByVal value As OpCodes)
                m_Opcode = Value
            End Set
        End Property

        Private m_Opcode As OpCodes
        Public Property Size() As UShort
            Get
                Return m_Size
            End Get
            Set(ByVal value As UShort)
                m_Size = Value
            End Set
        End Property

        Private m_Size As UShort

        Public Sub New(ByVal data As Byte(), Optional ByVal worldPacket As Boolean = True)
            MyBase.New(New MemoryStream(data))
            ' Packet header (0.5.3.3368): Size: 2 bytes + Cmd: 4 bytes
            If worldPacket Then
                Size = CUShort((Me.ReadUInt16() \ &H100) - 4)
                Opcode = Me.ReadUInt32()
            End If
        End Sub

        Public Function ReadInt8() As SByte
            Return MyBase.ReadSByte()
        End Function

        Public Shadows Function ReadInt16() As Short
            Return MyBase.ReadInt16()
        End Function

        Public Shadows Function ReadInt32() As Integer
            Return MyBase.ReadInt32()
        End Function

        Public Shadows Function ReadInt64() As Long
            Return MyBase.ReadInt64()
        End Function

        Public Function ReadUInt8() As Byte
            Return MyBase.ReadByte()
        End Function

        Public Shadows Function ReadUInt16() As UShort
            Return MyBase.ReadUInt16()
        End Function

        Public Shadows Function ReadUInt32() As UInteger
            Return MyBase.ReadUInt32()
        End Function

        Public Shadows Function ReadUInt64() As ULong
            Return MyBase.ReadUInt64()
        End Function

        Public Function ReadFloat() As Single
            Return MyBase.ReadSingle()
        End Function

        Public Shadows Function ReadDouble() As Double
            Return MyBase.ReadDouble()
        End Function

        Public Shadows Function ReadString(Optional ByVal terminator As Byte = 0) As String
            Dim tmpString As New StringBuilder()
            Dim tmpChar As Char = MyBase.ReadChar()
            Dim tmpEndChar As Char = Convert.ToChar(Encoding.UTF8.GetString(New Byte() {terminator}))

            While tmpChar <> tmpEndChar
                tmpString.Append(tmpChar)
                tmpChar = MyBase.ReadChar()
            End While

            Return tmpString.ToString()
        End Function

        Public Shadows Function ReadBytes(ByVal count As Integer) As Byte()
            Return MyBase.ReadBytes(count)
        End Function

        Public Function ReadStringFromBytes(ByVal count As Integer) As String
            Dim stringArray As Byte() = MyBase.ReadBytes(count)
            Array.Reverse(stringArray)

            Return Encoding.ASCII.GetString(stringArray)
        End Function

        Public Function ReadAccountName() As String
            Return ReadString(&HD).ToUpper()
        End Function

        Public Sub SkipBytes(ByVal count As Integer)
            MyBase.BaseStream.Position += count
        End Sub
    End Class


End Module