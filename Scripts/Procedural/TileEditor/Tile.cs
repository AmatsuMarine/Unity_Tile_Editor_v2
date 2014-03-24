using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
[System.Serializable]
public class Tile : MonoBehaviour {

	protected Tile_Map tile_map;

	// tiles to each side are stored here
	[SerializeField]
	public Tile tileRight, tileBelow, tileAbove, tileLeft;

	[SerializeField]
	protected int tileRow, tileCol;

	protected MeshFilter filter;
	protected MeshRenderer renderer;
	protected MeshCollider collider;

	protected Vector3 position;
	//protected Vector3 scale;

	void Awake(){
		filter = GetComponent<MeshFilter>();
		renderer = GetComponent<MeshRenderer>();
		collider = GetComponent<MeshCollider>();

		// Tiles need a basic mesh before it can be referenced by other tiles, so create the basic shape here
		filter.sharedMesh = startMesh();
	}

	// Use this for initialization
	void Start () {

		tile_map = transform.parent.GetComponent<Tile_Map>();

		renderer.materials = tile_map.tileSets;

		updateMesh();

		position = transform.position;
		//scale = transform.localScale;
	}

	public void updateMesh(){
		Mesh mesh = generateTileMesh();

		filter.sharedMesh = mesh;
		collider.sharedMesh = mesh;

		updateUV();
	}
	
	// Update if position was changed
	void Update () {
		if(transform.position != position){// || scale != transform.localScale){
			position = transform.position;
			//scale = transform.localScale;

			updateMesh();

			if(tileAbove)
				tileAbove.updateMesh();
			if(tileRight)
				tileRight.updateMesh();
			if(tileBelow)
				tileBelow.updateMesh();
			if(tileLeft)
				tileLeft.updateMesh();
		}
	}


	// allow chaining for setting tile data
	public Tile setTileRow(int row){
		tileRow = row;
		updateUV();

		return this;
	}

	public Tile setTileCol(int col){
		tileCol = col;
		updateUV();

		return this;
	}

	public Tile setTileRowCol(int row, int col){
		tileRow = row;
		tileCol = col;

		return this;
	}

	// go to the next tile in the row
	public Tile nextTile(){
		tileCol++;

		return this;
	}

	public int getTileRow(){ return tileRow; }
	public int getTileCol(){ return tileCol; }

	public void updateUV(){
		float uvWidth = 1f / tile_map.tileColumns;
		float uvHeight = 1f / tile_map.getTileRows();

		Vector2[] uv = filter.sharedMesh.uv;

		uv[0] = new Vector2(uvWidth * tileCol, uvHeight * tileRow);
		uv[1] = new Vector2(uvWidth * (tileCol + 1), uvHeight * tileRow);
		uv[2] = new Vector2(uvWidth * tileCol, uvHeight * (tileRow+1));
		uv[3] = new Vector2(uvWidth * (tileCol+1), uvHeight * (tileRow+1));



		filter.sharedMesh.uv = uv;

		filter.sharedMesh.RecalculateNormals();
		filter.sharedMesh.RecalculateBounds();
		filter.sharedMesh.Optimize();
	}

	// remove this tile from the surface
	public void remove(){
		if(tileAbove)
			tileAbove.tileBelow = null;
		if(tileBelow)
			tileBelow.tileAbove = null;
		if(tileLeft)
			tileLeft.tileRight = null;
		if(tileRight)
			tileRight.tileLeft = null;

		if(Application.isEditor)
			DestroyImmediate(this.gameObject);
		else
			Destroy(this.gameObject);
	}

	public Tile setRight(Tile tile){

		tileRight = tile;
		tile.tileLeft = this;

		if(tileAbove){
			if(tileAbove.tileRight){
				tile.tileAbove = tileAbove.tileRight;
				tileAbove.tileRight.tileBelow = tile;

				if(tileAbove.tileRight.tileRight){
					if(tileAbove.tileRight.tileRight.tileBelow){
						tileRight = tileAbove.tileRight.tileRight.tileBelow;
						tileAbove.tileRight.tileRight.tileBelow.tileLeft = tile;
					}
				}
			}
		}

		if(tileBelow){
			if(tileBelow.tileRight){
				tile.tileBelow = tileBelow.tileRight;
				tileBelow.tileRight.tileAbove = tile;

				if(tileBelow.tileRight.tileRight){
					if(tileBelow.tileRight.tileRight.tileAbove){
						tile.tileRight = tileBelow.tileRight.tileRight.tileAbove;
						tileBelow.tileRight.tileRight.tileAbove.tileLeft = tile;
					}
				}
			}
		}

		return tile;
	}

	public Tile setLeft(Tile tile){

		tileLeft = tile;
		tile.tileRight = this;

		if(tileAbove){
			if(tileAbove.tileLeft){
				tile.tileAbove = tileAbove.tileLeft;
				tileAbove.tileLeft.tileBelow = tile;

				if(tileAbove.tileLeft.tileLeft){
					if(tileAbove.tileLeft.tileLeft.tileBelow){
						tile.tileLeft = tileAbove.tileLeft.tileLeft.tileBelow;
						tileAbove.tileLeft.tileLeft.tileBelow.tileRight = tile;
					}
				}
			}
		}

		if(tileBelow){
			if(tileBelow.tileLeft){
				tile.tileBelow = tileBelow.tileLeft;
				tileBelow.tileLeft.tileAbove = tile;

				if(tileBelow.tileLeft.tileLeft){
					if(tileBelow.tileLeft.tileLeft.tileAbove){
						tileBelow.tileLeft.tileLeft.tileAbove.tileRight = tile;
						tile.tileLeft = tileBelow.tileLeft.tileLeft.tileAbove;
					}
				}
			}
		}

		return tile;
	}

	public Tile setAbove(Tile tile){

		tileAbove = tile;
		tile.tileBelow = this;

		if(tileLeft){
			if(tileLeft.tileAbove){
				tile.tileLeft = tileLeft.tileAbove;
				tileLeft.tileAbove.tileRight = tile;

				if(tileLeft.tileAbove.tileAbove){
					if(tileLeft.tileAbove.tileAbove.tileRight){
						tile.tileAbove = tileLeft.tileAbove.tileAbove.tileRight;
						tileLeft.tileAbove.tileAbove.tileRight.tileBelow = tile;
					}
				}
			}
		}

		if(tileRight){
			if(tileRight.tileAbove){
				tile.tileRight = tileRight.tileAbove;
				tileRight.tileAbove.tileLeft = tile;

				if(tileRight.tileAbove.tileAbove){
					if(tileRight.tileAbove.tileAbove.tileLeft){
						tile.tileAbove = tileRight.tileAbove.tileAbove.tileLeft;
						tileRight.tileAbove.tileAbove.tileLeft.tileBelow = tile;
					}
				}
			}
		}

		return tile;
	}

	public Tile setBelow(Tile tile){

		tileBelow = tile;
		tile.tileAbove = this;


		if(tileLeft.tileBelow){
			tile.tileLeft = tileLeft.tileBelow;
			tileLeft.tileBelow.tileRight = tile;

			if(tileLeft.tileBelow.tileBelow.tileRight){
				tileLeft.tileBelow.tileBelow.tileRight.tileAbove = tile;
				tile.tileBelow = tileLeft.tileBelow.tileBelow.tileRight;
			}
		}

		if(tileRight.tileBelow){
			tile.tileRight = tileRight.tileBelow;
			tileRight.tileBelow.tileLeft = tile;

			if(tileRight.tileBelow.tileBelow.tileLeft){
				tileRight.tileBelow.tileBelow.tileLeft.tileAbove = tile;
				tile.tileAbove = tileRight.tileBelow.tileBelow.tileLeft;
			}
		}

		return tile;
	}

	// <<-- CREATE A NEW TILE AND ADD IT TO THE SURFACE -->>
	public Tile addAbove(){
		Tile newTile = ((GameObject)Instantiate(gameObject, transform.position + new Vector3(0,0,1), transform.rotation)).GetComponent<Tile>();
		newTile.transform.parent = transform.parent;
		newTile.transform.localScale = transform.localScale;
		newTile.transform.position = transform.position + Vector3.Scale(Vector3.Scale(new Vector3(0,0,-1), transform.localScale), transform.parent.localScale);

		newTile.name = "Tile";

		newTile.tileAbove = null;
		newTile.tileBelow = null;
		newTile.tileRight = null;
		newTile.tileLeft = null;

		setAbove(newTile).generateTileMesh();

		return newTile;
	}

	public Tile addBelow(){
		Tile newTile = ((GameObject)Instantiate(gameObject, transform.position + new Vector3(0,0,-1), transform.rotation)).GetComponent<Tile>();
		newTile.transform.parent = transform.parent;
		newTile.transform.localScale = transform.localScale;
		newTile.transform.position = transform.position + Vector3.Scale(Vector3.Scale(new Vector3(0,0,1), transform.localScale), transform.parent.localScale);
		
		newTile.name = "Tile";

		newTile.tileAbove = null;
		newTile.tileBelow = null;
		newTile.tileRight = null;
		newTile.tileLeft = null;

		setBelow(newTile);

		newTile.generateTileMesh();

		return newTile;
	}

	public Tile addLeft(){
		Tile newTile = ((GameObject)Instantiate(gameObject, transform.position + new Vector3(1,0,0), transform.rotation)).GetComponent<Tile>();
		newTile.transform.parent = transform.parent;
		newTile.transform.localScale = transform.localScale;
		newTile.transform.position = transform.position + Vector3.Scale(Vector3.Scale(new Vector3(1,0,0), transform.localScale), transform.parent.localScale);
		
		newTile.name = "Tile";

		newTile.tileAbove = null;
		newTile.tileBelow = null;
		newTile.tileRight = null;
		newTile.tileLeft = null;

		setLeft(newTile).generateTileMesh();

		return newTile;
	}

	public Tile addRight(){
		Tile newTile = ((GameObject)Instantiate(gameObject, transform.position + new Vector3(-1,0,0), transform.rotation)).GetComponent<Tile>();
		newTile.transform.parent = transform.parent;
		newTile.transform.localScale = transform.localScale;
		newTile.transform.position = transform.position + Vector3.Scale(Vector3.Scale(new Vector3(-1,0,0), transform.localScale), transform.parent.localScale);
		
		newTile.name = "Tile";

		newTile.tileAbove = null;
		newTile.tileBelow = null;
		newTile.tileRight = null;
		newTile.tileLeft = null;

		setRight(newTile).generateTileMesh();

		return newTile;
	}

	// <<-- BUILD MESH -->>
	// generate a 2D square mesh of size 1
	protected virtual Mesh generateTileMesh(){

		Mesh mesh = new Mesh();

		mesh.vertices = calculateVertices();

		int[] mainTile = new int[]{
			0,1,2,
			2,1,3
		};

		int[] sides = new int[]{
			// above
			4,0,5,
			5,0,1,

			// right
			1,6,3,
			3,6,7,

			// below
			2,3,9,
			9,3,8,

			// left
			11,10,0,
			0,10,2
		};

		mesh.subMeshCount = 2;
		mesh.SetTriangles(mainTile,0);
		mesh.SetTriangles(sides,1);

		mesh.uv = new Vector2[]{
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(0,1),
			new Vector2(1,1),

			// bottom of tile
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),

			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f)
		};

		mesh.uv2 = new Vector2[]{
			new Vector2(0,1),
			new Vector2(1,1),
			new Vector2(1,1),
			new Vector2(0,1),

			// top
			new Vector2(0,0),
			new Vector2(1,0),

			// right
			new Vector2(1,0),
			new Vector2(0,0),

			// bottom
			new Vector2(0,0),
			new Vector2(1,0),

			// left
			new Vector2(1,0),
			new Vector2(0,0)
		};

		mesh.tangents = new Vector4[]{
			new Vector4(0,1,0,0),
			new Vector4(0,1,0,0),
			new Vector4(0,1,0,0),
			new Vector4(0,1,0,0),

			new Vector4(0,0,-1,0),
			new Vector4(0,0,-1,0),

			new Vector4(-1,0,0,0),
			new Vector4(-1,0,0,0),

			new Vector4(0,0,1,0),
			new Vector4(0,0,1,0),

			new Vector4(1,0,0,0),
			new Vector4(1,0,0,0)
		};

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		return mesh;
	}

	// calculate the position of the vertices based on the presence and location of neighboring tiles
	protected virtual Vector3[] calculateVertices(){
		Vector3[] vertices = new Vector3[]{
			new Vector3(0.5f,0,-0.5f),
			new Vector3(-0.5f,0,-0.5f),
			new Vector3(0.5f,0,0.5f),
			new Vector3(-0.5f,0,0.5f),
			
			// top (4-5)
			new Vector3(0.5f,0,-0.5f),
			new Vector3(-0.5f,0,-0.5f),
			
			// tileRight (6-7)
			new Vector3(-0.5f,0,-0.5f),
			new Vector3(-0.5f,0,0.5f),
			
			// bottom (8-9)
			new Vector3(-0.5f,0,0.5f),
			new Vector3(0.5f,0,0.5f),
			
			// tileLeft (10-11)
			new Vector3(0.5f,0,0.5f),
			new Vector3(0.5f,0,-0.5f)
		};
		
		if(tileAbove){
			Vector3 offset = tileAbove.transform.localPosition - transform.localPosition;

			vertices[4] = tileAbove.filter.sharedMesh.vertices[2] + offset;
			vertices[5] = tileAbove.filter.sharedMesh.vertices[3] + offset;
		}
		
		if(tileRight){
			Vector3 offset = tileRight.transform.localPosition - transform.localPosition;

			vertices[6] = tileRight.filter.sharedMesh.vertices[0] + offset;
			vertices[7] = tileRight.filter.sharedMesh.vertices[2] + offset;
		}
		
		if(tileBelow){
			Vector3 offset = tileBelow.transform.localPosition - transform.localPosition;

			vertices[8] = tileBelow.filter.sharedMesh.vertices[1] + offset;
			vertices[9] = tileBelow.filter.sharedMesh.vertices[0] + offset;
		}
		
		if(tileLeft){
			Vector3 offset = tileLeft.transform.localPosition - transform.localPosition;

			vertices[10] = tileLeft.filter.sharedMesh.vertices[3] + offset;
			vertices[11] = tileLeft.filter.sharedMesh.vertices[1] + offset;
		}

		return vertices;
	}


	// create a simple starting mesh
	protected Mesh startMesh(){
		Mesh mesh = new Mesh();
		
		mesh.vertices = new Vector3[]{
			new Vector3(0.5f,0,-0.5f),
			new Vector3(-0.5f,0,-0.5f),
			new Vector3(0.5f,0,0.5f),
			new Vector3(-0.5f,0,0.5f),
			
			// top
			Vector3.zero,
			Vector3.zero,
			
			// tileRight
			Vector3.zero,
			Vector3.zero,
			
			// bottom
			Vector3.zero,
			Vector3.zero,
			
			// tileLeft
			Vector3.zero,
			Vector3.zero
		};
		
		int[] mainTile = new int[]{
			0,1,2,
			2,1,3
		};

		int[] sides = new int[]{
			4,5,0,
			0,5,1,
			
			1,6,3,
			3,6,7,
			
			2,3,9,
			9,3,8,
			
			11,0,10,
			10,0,2
		};
		
		mesh.subMeshCount = 2;
		mesh.SetTriangles(mainTile,0);
		mesh.SetTriangles(sides,1);
		
		mesh.uv = new Vector2[]{
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(0,1),
			new Vector2(1,1),
			
			// bottom of tile
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.5f,0.5f)
		};
		
		mesh.uv2 = new Vector2[]{
			new Vector2(0,1),
			new Vector2(1,1),
			new Vector2(1,1),
			new Vector2(0,1),
			
			// top
			new Vector2(0,0),
			new Vector2(1,0),
			
			// tileRight
			new Vector2(1,0),
			new Vector2(0,0),
			
			//bottom
			new Vector2(0,0),
			new Vector2(1,0),
			
			// tileLeft
			new Vector2(1,0),
			new Vector2(0,0)
		};
		
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		return mesh;
	}

	public string[] getTileTypes(){
		return tile_map.getTypeNames();
	}
}
