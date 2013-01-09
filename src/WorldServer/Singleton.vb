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

Imports System.Collections
Imports System.Reflection


Public NotInheritable Class Singleton

    Private Sub New()
    End Sub


    Shared ObjectList As New Hashtable()
    Shared Sync As New [Object]()

    Public Shared Function GetInstance(Of T As Class)() As T
        Dim typeName = GetType(T).FullName

        SyncLock Sync
            If ObjectList.ContainsKey(typeName) Then
                Return DirectCast(ObjectList(typeName), T)
            End If
        End SyncLock

        Dim constructorInfo As ConstructorInfo = GetType(T).GetConstructor(BindingFlags.NonPublic Or BindingFlags.Instance, Nothing, Type.EmptyTypes, Nothing)
        Dim instance As T = DirectCast(constructorInfo.Invoke(New Object() {}), T)

        ObjectList.Add(instance.ToString(), instance)

        Return DirectCast(ObjectList(typeName), T)
    End Function



End Class