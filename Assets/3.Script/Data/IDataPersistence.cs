using System.Collections;
using System.Collections.Generic;

public interface IDataPersistence
{
    void LoadData(PlayerData data);

    void SaveData(ref PlayerData data);
}
