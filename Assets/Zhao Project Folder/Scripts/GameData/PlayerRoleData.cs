using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerRoleData
{
    public enum LevelTargetType
    {
        Heaven, Hell
    }
    public int roleModelID;
    public string roleName;
    public int roleAge;
    public bool roleSexIsM;
    public string roleRef;
    public string roleRef_Chinese;
    public int startNumber;
    public LevelTargetType levelTargetType;
    public int _ID;

    public PlayerRoleData(int roleModelID, string roleName, int roleAge, string roleRef, string roleRef_Chinese, int startNumber, LevelTargetType levelTargetType, int iD)
    {
        this.roleModelID = roleModelID;
        this.roleName = roleName;
        this.roleAge = roleAge;
        this.roleRef = roleRef;
        this.roleRef_Chinese = roleRef_Chinese;
        this.startNumber = startNumber;
        this.levelTargetType = levelTargetType;
        _ID = iD;
    }

    public PlayerRoleData() { }
}
