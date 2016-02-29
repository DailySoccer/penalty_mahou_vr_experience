using UnityEngine;
using System.Collections;

namespace FootballStar.Match3D {
	
	public class AnimatorID : MonoBehaviour {
		
		public static int Speed = 0;
		public static int Pasar = 0;
		public static int Chutar = 0;
		public static int Balon = 0;
		public static int Altura = 0;
		public static int Rematar = 0;
		public static int Distancia = 0;
		public static int Tiempo = 0;
		public static int AimAngle = 0;
		public static int Direction = 0;
		public static int AngularSpeed = 0;
		public static int Despejar = 0;
		public static int DireccionBalon = 0;
		public static int Celebracion = 0;
		public static int Derrota = 0;
		public static int SpeedFinal = 0;
		public static int MovingDirection = 0;
		public static int Lock = 0;
		public static int AxisZ = 0;
		public static int Regatear = 0;
		public static int Entrada = 0;
		public static int BallOut = 0;
		public static int Tecnica = 0;
		public static int DistanciaBalon = 0;
		public static int Pared = 0;
		public static int Parabola = 0;
		public static int DistanciaContext = 0;
		public static int DireccionContext = 0;
		public static int FrontContext = 0;
		public static int BackContext = 0;
		public static int LeftContext = 0;
		public static int RightContext = 0;
		public static int PotenciaContext = 0;
		public static int Unlink = 0;
		public static int Alerta = 0;
		public static int FinalPartido = 0;
		public static int MainCharacter = 0;
		public static int Tropezar = 0;
		
		public static int Idle = 0;
		public static int Front = 0;
		public static int Back = 0;
		public static int MoveLeft = 0;
		public static int MoveRight = 0;
		public static int Chut = 0;
		public static int Girar = 0;
		public static int Acelerar = 0;
		
		public static void Init () {
	        Speed 		= Animator.StringToHash("Speed");
			Pasar 		= Animator.StringToHash("Pasar");
	        Chutar 		= Animator.StringToHash("Chutar");
	        Balon 		= Animator.StringToHash("Balon");
	        Altura 		= Animator.StringToHash("Altura");
			Rematar 	= Animator.StringToHash("Rematar");
	        Distancia 	= Animator.StringToHash("Distancia");
	        Tiempo 		= Animator.StringToHash("Tiempo");
	        AimAngle 	= Animator.StringToHash("AimAngle");
			Direction 	= Animator.StringToHash("Direction");
	        AngularSpeed = Animator.StringToHash("AngularSpeed");
	        Despejar 	= Animator.StringToHash("Despejar");
	        DireccionBalon = Animator.StringToHash("DireccionBalon");
			Celebracion = Animator.StringToHash("Celebracion");
			Derrota 	= Animator.StringToHash("Derrota");
			SpeedFinal 	= Animator.StringToHash("SpeedFinal");
	        MovingDirection = Animator.StringToHash("MovingDirection");
	        Lock 		= Animator.StringToHash("Lock");
			AxisZ 		= Animator.StringToHash("AxisZ");
	        Regatear 	= Animator.StringToHash("Regatear");
	        Entrada 	= Animator.StringToHash("Entrada");
			BallOut 	= Animator.StringToHash("BallOut");
			Tecnica 	= Animator.StringToHash("Tecnica");
			DistanciaBalon 	= Animator.StringToHash("DistanciaBalon");
			Pared 		= Animator.StringToHash("Pared");
			Parabola	= Animator.StringToHash("Parabola");
			DistanciaContext	= Animator.StringToHash("DistanciaContext");
			DireccionContext	= Animator.StringToHash("DireccionContext");
			FrontContext	= Animator.StringToHash("FrontContext");
			BackContext		= Animator.StringToHash("BackContext");
			LeftContext		= Animator.StringToHash("LeftContext");
			RightContext	= Animator.StringToHash("RightContext");
			PotenciaContext	= Animator.StringToHash("PotenciaContext");
			Unlink 		= Animator.StringToHash("Unlink");
			Alerta		= Animator.StringToHash("Alerta");
			FinalPartido	= Animator.StringToHash("FinalPartido");
			MainCharacter	= Animator.StringToHash("MainCharacter");
			Tropezar	= Animator.StringToHash("Tropezar");
			
			Idle		= Animator.StringToHash("Idle");
			Front		= Animator.StringToHash("Front");
			Back		= Animator.StringToHash("Back");
			MoveLeft	= Animator.StringToHash("MoveLeft");
			MoveRight	= Animator.StringToHash("MoveRight");
			Chut		= Animator.StringToHash("Chut");
			Girar		= Animator.StringToHash("Girar");
			Acelerar	= Animator.StringToHash("Acelerar");
		}
		
		public static void AnimationReset<T>( Animator anim, string parameter ) {
			if ( typeof(T) == typeof(bool) )
				anim.SetBool( parameter, false );
			else if ( typeof(T) == typeof(float) )
				anim.SetFloat ( parameter, 0 );
			else if ( typeof(T) == typeof(int) )
				anim.SetInteger( parameter, 0 );
		}
		
		public static void AnimationReset( Animator anim ) {
	        AnimationReset<float>( anim, "Speed" );
			AnimationReset<bool>( anim, "Pasar" );
	        AnimationReset<bool>( anim, "Chutar" );
	        // AnimationReset<float>( anim, "Balon" );
	        AnimationReset<float>( anim, "Altura" );
			AnimationReset<bool>( anim, "Rematar" );
	        AnimationReset<float>( anim, "Distancia" );
	        AnimationReset<float>( anim, "Tiempo" );
	        AnimationReset<float>( anim, "AimAngle" );
			AnimationReset<float>( anim, "Direction" );
	        AnimationReset<float>( anim, "AngularSpeed" );
	        AnimationReset<bool>( anim, "Despejar" );
	        AnimationReset<float>( anim, "DireccionBalon" );
			AnimationReset<int>( anim, "Celebracion" );
			AnimationReset<int>( anim, "Derrota" );
	        AnimationReset<float>( anim, "SpeedFinal" );
	        AnimationReset<int>( anim, "MovingDirection" );
	        // AnimationReset<float>( anim, "Lock" );
			//vAnimationReset<float>( anim, "AxisZ" );
	        AnimationReset<bool>( anim, "Regatear" );
	        AnimationReset<bool>( anim, "Entrada" );
			// AnimationReset<float>( anim, "BallOut" );
			AnimationReset<int>( anim, "Tecnica" );
			AnimationReset<float>( anim, "DistanciaBalon" );
			AnimationReset<bool>( anim, "Pared" );
			AnimationReset<float>( anim, "Parabola" );
			AnimationReset<int>( anim, "DistanciaContext" );
			AnimationReset<int>( anim, "DireccionContext" );
			AnimationReset<bool>( anim, "FrontContext" );
			AnimationReset<bool>( anim, "BackContext" );
			AnimationReset<bool>( anim, "LeftContext" );
			AnimationReset<bool>( anim, "RightContext" );
			AnimationReset<int>( anim, "PotenciaContext" );
			// AnimationReset<float>( anim, "Unlink" );
			AnimationReset<bool>( anim, "Alerta" );
			AnimationReset<bool>( anim, "FinalPartido" );
			AnimationReset<bool>( anim, "MainCharacter" );
			AnimationReset<bool>( anim, "Tropezar" );
		}
		
		public static string AnimationParameter<T>( Animator anim, string parameter ) {
			string str = "";
			if ( typeof(T) == typeof(bool) )
				str = System.String.Format ( "{0}: {1}", parameter, anim.GetBool(parameter) );
			else if ( typeof(T) == typeof(float) )
				str = System.String.Format ( "{0}: {1}", parameter, anim.GetFloat(parameter) );
			else if ( typeof(T) == typeof(int) )
				str = System.String.Format ( "{0}: {1}", parameter, anim.GetInteger(parameter) );
			else
				str = System.String.Format ( "{0}: ???", parameter );
			return str;
		}
		
		public static void Log ( GameObject gameObject ) {
			Animator anim = gameObject.GetComponent<Animator>();
			if ( anim ) {
				string texto = "";
				texto += System.String.Format ( "{0}: Anim Parameters -----\n", gameObject.name );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "Speed") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<bool>(anim, "Pasar") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<bool>(anim, "Chutar") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "Balon") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "Altura") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<bool>(anim, "Rematar") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "Distancia") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "Tiempo") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "AimAngle") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "Direction") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "AngularSpeed") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<bool>(anim, "Despejar") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "DireccionBalon") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<int>(anim, "Celebracion") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "SpeedFinal") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<int>(anim, "MovingDirection") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "Lock") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "AxisZ") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<bool>(anim, "Regatear") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<bool>(anim, "Entrada") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "BallOut") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<int>(anim, "Tecnica") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "DistanciaBalon") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<bool>(anim, "Pared") );
				texto += System.String.Format ( "{0}\n", AnimationParameter<float>(anim, "Parabola") );
				Debug.Log ( texto );
			}
		}
	}
	
}

