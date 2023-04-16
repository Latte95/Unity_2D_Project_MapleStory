using System.Collections;
using System.Collections.Generic;

public interface IDataPersistence
{
    void LoadData(Player data);

    void SaveData(ref Player data);
}
