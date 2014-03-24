using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Tile))]
[CanEditMultipleObjects]
public class TileEdit : Editor {

	int newTileDirection = 0;
	private Tile[] tiles;
	
	public void OnEnable(){
		tiles = new Tile[targets.Length];
		for(int i = 0; i < tiles.Length; i++){
			tiles[i] = (Tile)targets[i];
		}
	}
	
	public override void OnInspectorGUI(){
		int tileRow = EditorGUILayout.Popup(tiles[0].getTileRow(), tiles[0].getTileTypes());
		for(int i = 0; i < tiles.Length; i++){
			tiles[i].setTileRow(tileRow);
		}

		if(GUILayout.Button("Next Texture")){
			for(int i = 0; i < tiles.Length; i++){
				tiles[i].nextTile();
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		newTileDirection = EditorGUILayout.Popup(newTileDirection, new string[]{"Above", "Below", "Left", "Right"});

		if(GUILayout.Button("Add Tile")){
			if(newTileDirection == 0){
				for(int i = 0; i < tiles.Length; i++){
					tiles[i].addAbove();
				}
			}
			else if(newTileDirection == 1){
				for(int i = 0; i < tiles.Length; i++){
					tiles[i].addBelow();
				}
			}
			else if(newTileDirection == 2){
				for(int i = 0; i < tiles.Length; i++){
					tiles[i].addLeft();
				}
			}
			else{
				for(int i = 0; i < tiles.Length; i++){
					tiles[i].addRight();
				}
			}
		}

		if(GUILayout.Button("Remove Tile")){
			for(int i = 0; i < tiles.Length; i++){
				tiles[i].remove();
			}
		}
	}
}