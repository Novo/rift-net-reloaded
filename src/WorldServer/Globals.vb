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

Public Class Globals
    'Public Shared DataMgr As DataManager
    'Public Shared ObjectMgr As ObjectManager
    'Public Shared SkillMgr As SkillManager
    'Public Shared SpawnMgr As SpawnManager
    'Public Shared SpellMgr As SpellManager
    Public Shared WorldMgr As WorldManager


    Public Shared Sub InitializeManager()
        'DataMgr = DataManager.GetInstance()
        'ObjectMgr = ObjectManager.GetInstance()
        'SkillMgr = SkillManager.GetInstance()
        'SpawnMgr = SpawnManager.GetInstance()
        'SpellMgr = SpellManager.GetInstance()
        WorldMgr = WorldManager.GetInstance()
    End Sub



End Class