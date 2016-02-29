using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FootballStar.Match3D {
	
	class Cell {
		public float InfluenceLocal;
		public float InfluenceVisitante;
		
		public float InfluenceTotal {
			get { return InfluenceLocal + InfluenceVisitante; }
		}
		public float DifferenceLocal { 
			get { return InfluenceLocal - InfluenceVisitante; }
		}
		public float DifferenceVisitante { 
			get { return InfluenceVisitante - InfluenceLocal; }
		}
	};
	
	class CellXZ : Cell {
		public int x = -1;
		public int z = -1;
	};
	
	public class InfluenceMap : MonoBehaviour {
	
		public int Width = 22;
		public int Height = 13;
		public float CellDistance = 5f;
		float iniX;
		float iniZ;
		
		Cell[] cells;
		List<SoccerData> jugadoresLocales;
		List<SoccerData> jugadoresVisitantes;
		
		public void Setup () {
			cells = new Cell[ Width * Height ];
			for( int i=0; i< (Width * Height); i++ )
				cells[i] = new Cell();
			
			jugadoresLocales = new List<SoccerData>();
			jugadoresVisitantes = new List<SoccerData>();
			
			iniX = -(Width / 2) * CellDistance;
			iniZ = -(Height / 2) * CellDistance;
			
			Entrenador[] entrenadores = GameObject.FindObjectsOfType( typeof(Entrenador) ) as Entrenador[];
			foreach ( Entrenador entrena in entrenadores ) {
				SoccerData data = entrena.Target.GetComponentInChildren<SoccerData>();
				if ( entrena.tag == "Local" )
					jugadoresLocales.Add ( data );
				else
					jugadoresVisitantes.Add ( data );
			}

			StopAllCoroutines();
			StartCoroutine( UpdateInfo () );
		}
		
		Cell CellFromXZ( int x, int z ) {
			if ( x >= 0 && x < Width && z >= 0 && z < Height ) {
				int index = (int) ((z * Width) + x);
				return cells[ index ];
			}
			return null;
		}
		
		Vector3 CellToPosition( int index ) {
			return CellToPosition( index % Width, (int)(index / Width) );
		}
		
		Vector3 CellToPosition( int x, int z ) {
			return new Vector3( iniX + (x * CellDistance), 0, iniZ + (z * CellDistance) );
		}
		
		int PositionToCellIndex( Vector3 pos ) {
			int x = (int) ( (pos.x - iniX + CellDistance * 0.5f) / CellDistance);
			x = Mathf.Clamp( x, 0, Width-1 );
			
			int z = (int) ( (pos.z - iniZ + CellDistance * 0.5f) / CellDistance);
			z = Mathf.Clamp( z, 0, Height-1 );
			
			int index = (int) ((z * Width) + x);
			// Assert.Test ( ()=> { return (CellToPosition(index) == CellToPosition(x, z)); }, "Error" );
			Assert.Test( ()=> { return (index>=0)&&(index<Width*Height); }, "Celda invalida" );
			return index;
		}
		
		Cell PositionToCell( Vector3 pos ) {
			int index = PositionToCellIndex( pos );
			return cells[index];
		}
		
		float CalculateFactor( SoccerData data ) {
			return 1;
		}
		
		void ClearCells() {
			foreach( Cell cell in cells ) {
				cell.InfluenceLocal = 0;
				cell.InfluenceVisitante = 0;
			}
		}
		
		public void UpdateInfluence() {
			ClearCells ();
			
			foreach( SoccerData local in jugadoresLocales ) {
				// El propietario del balon tiene mas influencia
				float factor = (local.BallPropietary) ? 2f : 1f;
				
				Cell cell = PositionToCell( local.transform.position );
				cell.InfluenceLocal += CalculateFactor( local ) * factor;
				
				if ( local.BallPropietary ) {
					Cell left = PositionToCell( local.transform.position + Vector3.left * 2f );
					if ( left != null )
						left.InfluenceLocal += CalculateFactor( local ) * 0.5f * factor;
					Cell right = PositionToCell( local.transform.position + Vector3.right * 2f );
					if ( right != null )
						right.InfluenceLocal += CalculateFactor( local ) * 0.5f * factor;
					Cell up = PositionToCell( local.transform.position + Vector3.up * 2f );
					if ( up != null )
						up.InfluenceLocal += CalculateFactor( local ) * 0.5f * factor;
					Cell down = PositionToCell( local.transform.position + Vector3.down * 2f );
					if ( down != null )
						down.InfluenceLocal += CalculateFactor( local ) * 0.5f * factor;
				}
			}
			
			foreach( SoccerData visitante in jugadoresVisitantes ) {
				Cell cell = PositionToCell( visitante.transform.position );
				cell.InfluenceVisitante += CalculateFactor( visitante );
				
				if ( visitante.BestCellPosition.y < Vector3.up.y ) {
					Cell cellNext = PositionToCell( visitante.BestCellPosition );
					cellNext.InfluenceVisitante += CalculateFactor( visitante );
				}
				
				/*
				Cell left = PositionToCell( visitante.transform.position + Vector3.left * 2f );
				if ( left != null )
					left.InfluenceVisitante += CalculateFactor( visitante ) * 0.5f;
				Cell right = PositionToCell( visitante.transform.position + Vector3.right * 2f );
				if ( right != null )
					right.InfluenceVisitante += CalculateFactor( visitante ) * 0.5f;
				Cell up = PositionToCell( visitante.transform.position + Vector3.up * 2f );
				if ( up != null )
					up.InfluenceVisitante += CalculateFactor( visitante ) * 0.5f;
				Cell down = PositionToCell( visitante.transform.position + Vector3.down * 2f );
				if ( down != null )
					down.InfluenceVisitante += CalculateFactor( visitante ) * 0.5f;
				*/
			}
		}
		
		IEnumerator UpdateInfo() {
			while ( true ) {
				UpdateInfluence();
				yield return new WaitForSeconds( 1f );
			}
		}
		
		Vector3 BestPositionLocal( SoccerData data ) {
			// Cell cell = PositionToCell( data.gameObject.transform.position );
			return Vector3.up;
		}
		
		CellXZ BestCellVisitante( CellXZ best, int x, int z ) {
			Cell cell = CellFromXZ( x, z );
			if ( cell != null ) {
				if ( (cell.InfluenceLocal > 0) && (cell.InfluenceLocal >= best.InfluenceLocal) && (cell.DifferenceVisitante < best.DifferenceVisitante) ) {
					best.InfluenceLocal = cell.InfluenceLocal;
					best.InfluenceVisitante = cell.InfluenceVisitante;
					best.x = x;
					best.z = z;
				}
			}
			return best;
		}
		
		Vector3 BestPositionVisitante( SoccerData data ) {
			int index = PositionToCellIndex( data.gameObject.transform.position );
			Cell cell = cells[index];

			// Puede abandonar su posición?
			if ( cell.DifferenceVisitante >= 1f ) {
				
				int x = index % Width;
				int z = index / Width;
				
				CellXZ best = new CellXZ();
			
				best.InfluenceLocal = cell.InfluenceLocal;
				best.InfluenceVisitante = cell.InfluenceVisitante;
				best.x = x;
				best.z = z;
				
				// Mirar celdas de alrededor
				best = BestCellVisitante ( best, x-1, z );
				best = BestCellVisitante ( best, x+1, z );
				best = BestCellVisitante ( best, x, z-1 );
				best = BestCellVisitante ( best, x, z+1 );

				best = BestCellVisitante ( best, x-1, z-1 );
				best = BestCellVisitante ( best, x-1, z+1 );
				best = BestCellVisitante ( best, x+1, z-1 );
				best = BestCellVisitante ( best, x+1, z+1 );
				
				if ( best.x != x || best.z != z ) {
					// Modificamos la influencia en la celda para hacerla menos deseable
					/*
					Cell cellBest = CellFromXZ ( best.x, best.z );
					cellBest.InfluenceVisitante += 0.5f;
					cell.InfluenceVisitante -= 0.5f;
					*/
					UpdateInfluence();
					
					return CellToPosition( best.x, best.z );
				}
			}
			
			return Vector3.up;
		}
		
		public Vector3 BestPosition( SoccerData data ) {
			// return (data.Bando == SoccerData.EBando.Local) ? BestPositionLocal( data ) : BestPositionVisitante( data );
			return Vector3.up;
		}
	
		void Start () {
		}
		
		void Update () {
		}
		
		void OnDrawGizmos () {
			/*
			if ( cells != null ) {
				for ( int z=0; z<Height; z++ ) {
					for ( int x=0; x<Width; x++ ) {
						int index = (z * Width) + x;
						Cell cell = cells[index];
						
						Vector3 pos = CellToPosition( x, z );
						if ( cell.InfluenceTotal > 0f ) {
							Gizmos.color = ( cell.DifferenceLocal > 0 ) ? Color.green : Color.red;
							Gizmos.DrawWireSphere( pos, CellDistance * 0.5f );
						}
						else {
							Gizmos.color = Color.black;
							Gizmos.DrawWireSphere( pos, 1 );
						}
					}
				}
			}
			*/
		}		
	}
	
}
