using UnityEngine;
using System.Collections;

public class Tile_Map : MonoBehaviour {

	public Material[] tileSets = null;
	
	public int tileColumns = 1;

	public string[] typeNames = {"Null"};

	public int getTileRows(){
		return typeNames.Length;
	}

	public int getTileColumns(){
		return tileColumns;
	}

	public string[] getTypeNames(){
		return typeNames;
	}

	public void setTileRows(int r){
		string[] oldTypes = typeNames;

		typeNames = new string[r];


		for(int i = 0; i < typeNames.Length; i++){
			if(i < oldTypes.Length)
				typeNames[i] = oldTypes[i];
			else
				typeNames[i] = "Tile";
		}
	}

	public void setTileCols(int c){
		tileColumns = c;
	}

	public void setTypeNames(params string[] names){
		typeNames = names;
	}
}
