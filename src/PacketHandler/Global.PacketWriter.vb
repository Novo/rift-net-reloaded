Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text

    Public Class PacketWriter
    Inherits BinaryWriter

        Public Property Opcode() As Opcodes
        Get
        End Get
        Set(ByVal value As OpCodes)
        End Set
    End Property

    Public Property Size() As UShort
        Get
        End Get
        Set(ByVal value As UShort)
        End Set
    End Property

        Public Sub New()
            MyBase.New(New MemoryStream())
        End Sub

    Public Sub New(ByVal opcode As OpCodes, ByVal length As Byte, Optional ByVal isWorldPacket As Boolean = True)
        MyBase.New(New MemoryStream())
        Me.Opcode = Opcode
        WritePacketHeader(Opcode, length, isWorldPacket)
    End Sub

    Protected Sub WritePacketHeader(ByVal opcode As OpCodes, ByVal length As Byte, Optional ByVal isWorldPacket As Boolean = True)
        ' Packet header (0.5.3.3368): Size: 2 bytes + Cmd: 2 bytes
        ' Packet header after SMSG_AUTH_CHALLENGE (0.5.3.3368): Size: 2 bytes + Cmd: 4 bytes
        WriteUInt8(0)
        If isWorldPacket Then
            WriteUInt8(CByte(length + 4))
        Else
            WriteUInt8(CByte(length + 2))
        End If

        WriteUInt8(CByte(CUInt(opcode) Mod &H100))
        WriteUInt8(CByte(CUInt(opcode) \ &H100))

        If isWorldPacket Then
            WriteUInt8(0)
            WriteUInt8(0)
        End If
    End Sub

        Public Function ReadDataToSend() As Byte()
            Dim data As Byte() = New Byte(BaseStream.Length - 1) {}
            Seek(0, SeekOrigin.Begin)

            For i As Integer = 0 To BaseStream.Length - 1
                data(i) = CByte(BaseStream.ReadByte())
            Next

            Return data
        End Function

        Public Sub WriteInt8(ByVal data As SByte)
            MyBase.Write(data)
        End Sub

        Public Sub WriteInt16(ByVal data As Short)
            MyBase.Write(data)
        End Sub

        Public Sub WriteInt32(ByVal data As Integer)
            MyBase.Write(data)
        End Sub

        Public Sub WriteInt64(ByVal data As Long)
            MyBase.Write(data)
        End Sub

        Public Sub WriteUInt8(ByVal data As Byte)
            MyBase.Write(data)
        End Sub

        Public Sub WriteUInt16(ByVal data As UShort)
            MyBase.Write(data)
        End Sub

        Public Sub WriteUInt32(ByVal data As UInteger)
            MyBase.Write(data)
        End Sub

        Public Sub WriteUInt64(ByVal data As ULong)
            MyBase.Write(data)
        End Sub

        Public Sub WriteFloat(ByVal data As Single)
            MyBase.Write(data)
        End Sub

        Public Sub WriteDouble(ByVal data As Double)
            MyBase.Write(data)
        End Sub

    Public Sub WriteString(ByVal data As String)
        Dim sBytes As Byte() = Encoding.ASCII.GetBytes(data)
        Me.WriteBytes(sBytes)
        MyBase.Write(CByte(0)) 'String null terminated
    End Sub

        Public Sub WriteBytes(ByVal data As Byte())
            MyBase.Write(data)
        End Sub
    End Class