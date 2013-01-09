'Rift .NET Reloaded -- An OpenSource Server Emulator for World of Warcraft Classic Alpha 0.5.3 (3368) written in VB.Net
'Copyright (c) 2013 noVo aka. takeoYasha www.easy-emu.de

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

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Data.SQLite
Imports System.Data


Public Class SQLiteBase
    Implements IDisposable

    Private Connection As SQLiteConnection
    Private SqlData As SQLiteDataReader

    Private m_RowCount As Integer

    Public Property RowCount() As Integer
        Get
            Return m_RowCount
        End Get

        Set(ByVal value As Integer)
            m_RowCount = value
        End Set
    End Property


    Public Sub New(ByVal db As String)
        Connection = New SQLiteConnection("Data Source=Database/" & db & ".s3db; Version=3")
        'Connection = New SQLiteConnection("URI=file:Database/" & db & ".s3db; Version=3") 'Mono fix?

        Try
            Connection.Open()
        Catch ex As SQLiteException
            Console.WriteLine("[{0}] Error Opening Database:" & Environment.NewLine & "{1}", Format(TimeOfDay, "HH:mm:ss"), ex.ToString)
        End Try
    End Sub


    Public Sub DisposeDatabaseConnection() Implements System.IDisposable.Dispose
        Try
            SqlData.Close()
            Connection.Close()
        Catch
        End Try
    End Sub


    Public Function Execute(ByVal sql As String, ByVal ParamArray args As Object()) As Boolean
        Dim sqlString As New StringBuilder()

        Try
            sqlString.AppendFormat(sql, args)
        Catch ex As Exception
            Console.WriteLine("[{0}] Error Building SQL Command: {1}" & Environment.NewLine & "{2}", Format(TimeOfDay, "HH:mm:ss"), sql, ex.ToString)
        End Try


        Dim sqlCommand As New SQLiteCommand(sqlString.ToString, Connection)

        Try
            sqlCommand.ExecuteNonQuery()
            Return True
        Catch ex As SQLiteException
            Console.WriteLine("[{0}] Error Execute SQL Command: {1}" & Environment.NewLine & "{2}", Format(TimeOfDay, "HH:mm:ss"), sqlString.ToString, ex.ToString)
            Return False
        End Try
    End Function


    Public Function [Select](ByVal sql As String) As DataTable
        Dim retData As New DataTable()
        Dim sqlCommand As New SQLiteCommand(sql, Connection)

        Try
            SqlData = sqlCommand.ExecuteReader(CommandBehavior.[Default])
            retData.Load(SqlData)

        Catch ex As SQLiteException
            Console.WriteLine("[{0}] Error Building SQL Command: {1}" & Environment.NewLine & "{2}", Format(TimeOfDay, "HH:mm:ss"), sql, ex.ToString)
        End Try

        Return retData
    End Function



End Class