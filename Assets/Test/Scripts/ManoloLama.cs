using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using FootballStar.Match3D;
using FootballStar.Common;

public enum InteractiveType
{
    None,    
    Dribling,
    Pass,
    Shot,
    Init,
    Middle,
    End,
    All,
};

public class ManoloLama : MonoBehaviour {

    public class Momento
    {
        public int mTiempo;
        public bool mLocal;
        public InteractiveType mType;
        public bool mUser;
        public bool mExito;
        public int mNivel;
        public float mRandom;

        public Momento( int tiempo, InteractiveType type= InteractiveType.None) {
            mTiempo = tiempo;
            mType = type;
            mLocal = false;
            mUser = false;
            mExito = false;
            mNivel = 0;
            mRandom = 0;
        }

        public override string ToString()
        {
            return string.Format("t: {0} {1} {2} {3}", mTiempo, mType, mLocal?"Local":"Visitante", mUser ? "MAN" : "AUT");
        }
    }
    List<Momento> mMoments = new List<Momento>();

    public static ManoloLama Instance; 

    public GameObject Panel;
    public float Size = 0.1f;
    public int max = 8;    
    public bool Skipping { get; set; }

    bool mFinPartido = false;
    bool mSegundaParte = false;

    MatchManager mMatchManager;
    MatchBridge mMatchBridge;
    public InteractiveType mInteractiveType = InteractiveType.None;
    List<Momento>.Enumerator mMomentIT;

    void Awake()
    {
        Instance = this;
    }

    public void Init() {
        var gameModel = GameObject.FindGameObjectWithTag("MatchManager");
        mMatchManager = gameModel.GetComponent<MatchManager>();
        mMatchBridge = GameObject.FindGameObjectWithTag("GameModel").GetComponent<MatchBridge>();

        Skipping = false;

        // Se determina el número de interacciones del jugador
        var interactivas = Random.Range(3, 6);
        // Se determina el número de interacciones exitosas requeridas.
        var exitos = Random.Range(1, interactivas);
        // Se calculan los goles contrarios totales = interacciones exitosas requeridas -1
        var golesContrarios = exitos - 1;
        // Se determinan los pasos de narración y sus minutos(min por determinar, máx 30, cada 3 - 5 minutos)
        mMoments.Clear();
        int minuto = 0;
        mMoments.Add(new Momento(minuto, InteractiveType.Init));
        do
        {
            minuto += Random.Range(3, 5);
            mMoments.Add(new Momento(minuto, InteractiveType.None));
            if (minuto >= 45 && !mSegundaParte) {
                mSegundaParte = true;
                minuto = 45;
                mMoments.Add(new Momento(minuto, InteractiveType.Middle));
            }
            else if (minuto > 90 && !mFinPartido) {
                mMoments.Add(new Momento(minuto, InteractiveType.End));
                break;
            }
        } while (true);
        //////////////////////////////////////////////////////
        var total = mMoments.Count - 3; // Quito el inicio, medio tiempo y final.
        // Se determina en qué pasos marcarán los contrarios
        MakeMoments(golesContrarios, InteractiveType.Shot, false, false, true);
        // Se determina en qué pasos de la narración saltan las interacciones de usuario, sabiendo que tiene que haber al menos un paso de narración libre después de cada una de las interacciones.
        MakeMoments(interactivas, InteractiveType.All, true, true, false);
        // El resto se ponen como interacciones no efectivas.
        var resto = total - golesContrarios - interactivas;
        var tmp = resto / 2;
        MakeMoments(tmp, InteractiveType.All, false, false, false);
        resto -= tmp;
        MakeMoments(resto, InteractiveType.All, true, false, false);

        // Para cada interacción de usuario
        // Me quedan oportunidades (restantes >= exitosas requeridas) 
        // Decido si es oportunidad de gol.
        // SI no es oportunidad de gol, y resuelvo con éxito, reservo slot de tiempo posterior para marcar gol en narración
        // Si no me quedan oportunidades, no puede ser oportunidad de gol y no reservo slot para marcar gol en la narración aunque la haga con éxito.

        foreach (var moment in mMoments) {
            Debug.Log(">>>>> " + moment );
        }
        mSegundaParte = false;
        mMomentIT = mMoments.GetEnumerator();
    }

    void MakeMoments(int moments, InteractiveType type, bool local, bool user, bool success)
    {
        List<Momento> freeMom = null;
        freeMom = mMoments.FindAll(e => e.mType == InteractiveType.None);
        bool oneShot = false;
        while (moments > 0) {
            int idx = Random.Range(0, freeMom.Count - 1);
            freeMom[idx].mLocal = local;
            freeMom[idx].mUser = user;

            if (type != InteractiveType.All) {
                freeMom[idx].mType = type;
            } else {
                if (moments == 1 && !oneShot)
                    freeMom[idx].mType = InteractiveType.Shot;
                else {
                    freeMom[idx].mType = (InteractiveType)Random.Range((int)InteractiveType.Dribling, (int)InteractiveType.Shot + 1);
                    if (freeMom[idx].mType == InteractiveType.Shot) oneShot = true;
                }
            }

            freeMom[idx].mExito = success;
            freeMom.RemoveAt(idx);
            moments--;
        }
    }

    Momento getFreeMoment() {
        List<Momento> free = mMoments.FindAll(e => e.mType == InteractiveType.None);
        return free[ Random.Range(0,free.Count-1) ];
    }

    System.Text.StringBuilder StringBuilder = new System.Text.StringBuilder();
    // Use this for initialization
    void InsertPanel(Momento cur, bool reevaluate) {
        string tiempo="";
        if (cur.mTiempo > 90 || (cur.mTiempo > 45 && !mSegundaParte)){
            if (cur.mTiempo > 90) {
                tiempo = string.Format("90+{0}", (cur.mTiempo - 90));
            }
            else {
                tiempo = string.Format("45+{0}", (cur.mTiempo - 45));
            }
        }
        else {
            tiempo = string.Format("{0}", cur.mTiempo);
        }

        NarratorMessageType type;
        if (cur.mType >= InteractiveType.Init) {
            type = NarratorMessageType.Descriptive;
        }
        else {
            type = cur.mLocal ? NarratorMessageType.LocalPlayerMessage : NarratorMessageType.VisitantPlayerMessage;
        }
        string action = "";
        string reaction = "";
        if(!reevaluate) cur.mRandom = cur.mUser?0.0f:Random.value;
        switch (cur.mType)
        {
            case InteractiveType.Init:
                action= "Comienza el partido.";
                break;
            case InteractiveType.Middle:
                action = "Comienza la segunda parte.";
                break;
            case InteractiveType.End:
                action = "Finaliza el partido.";
                break;
            case InteractiveType.Pass:
                if (cur.mRandom < 0.5f)
                { // Pase
                    action = Response( "Pase entre líneas del {0}.\n", "{0} intenta un pase.\n");
                    if (!cur.mExito) {
                        reaction = Response("Pero el {1} la corta.", "Pero se va fuera.", "Y se la entrega a un contrario.");
                    }
                    else {
                        reaction = Response("Y continúa la jugada.", "Y se acerca a puerta.", "Y encara a la defensa.");
                    }
                }
                else
                { // Saque
                    action = Response("El {0} saca de banda.\n", "Saque de banda del {0}.\n");
                    if (!cur.mExito)
                    {
                        reaction = Response("Pero recupera el {1}.", "Pero el {1} corta el saque.");
                    }
                    else {
                        reaction = Response("Y controla el balón. ", "Y monta el ataque.");
                    }
                }
                break;

            case InteractiveType.Shot:
                if (cur.mRandom < 0.33f)
                {// TIRO
                    action = Response("{0} tira a puerta.\n", "{0} lo intenta desde fuera del área.\n", "Disparo a bocajarro del {0}.\n");
                    if (!cur.mExito)
                    {
                        reaction = Response("Pero el portero contrario la para.", "Pero se va fuera.", "Sale fuera cerca del poste.");
                    }
                    else {
                        reaction = Response("Por las escuadra… GOOOOL!", "Ajustado al palo… GOOOOL!", "GOOOOL! El portero no pudo hacer nada.");
                    }
                }
                else if (cur.mRandom < 0.66f)
                { // ASISTENCIAS
                    action = Response("Pase entre líneas del {0}.\n", "Gran asistencia del {0}.\n" );
                    if (!cur.mExito)
                    {
                        reaction=Response("Pero el {1} intercepta el pase.", "Pero el {1} logra cortar.");
                    }
                    else {
                        reaction = Response("Que logra rematar y GOOOOL!", "Que controla, tira y GOOOOL!");
                    }
                }
                else
                { // CÓRNER 
                    action = "El {0} saca de esquina\n";
                    if (!cur.mExito) {
                        reaction = Response("Pero el {1} despeja el peligro.", "Pero el balón se va fuera.");
                    }
                    else {
                        reaction = Response("GOOOL! Gran remate de cabeza.", "Controla dentro del área y GOOOOL!");
                    }
                }
                break;
            case InteractiveType.Dribling:
                action = Response("Intento de regate del {0}.\n", "El extremo del {0} intenta un regate.\n", "El centrocampista del {0} intenta un regate.\n");
                if (!cur.mExito) {
                    reaction=Response("Y no lo consigue.", "Y pierde el balón.");
                }
                else {
                    reaction = Response("Gran regate, se queda solo.", "Lo consigue, pueden encarar.");
                }
                break;
        }

        string a = cur.mLocal ? mMatchBridge.CurrentMatchDefinition.MyName : mMatchBridge.CurrentMatchDefinition.OpponentName;
        string b = cur.mLocal ? mMatchBridge.CurrentMatchDefinition.OpponentName: mMatchBridge.CurrentMatchDefinition.MyName;

        StringBuilder.Length = 0;
        StringBuilder.Append(action);
        if(!cur.mUser) StringBuilder.Append(reaction);

        NarratorUI.Instance.AddMessage(tiempo, string.Format(StringBuilder.ToString(), a, b) , cur.mExito && cur.mType == InteractiveType.Shot, type, reevaluate);
    }

    string Response(string r1, string r2) { return Random.value > 0.5f ? r1 : r2; }
    string Response(string r1, string r2, string r3) { return Random.value < 0.33f ? r1 : Random.value < 0.66f ? r2: r3; }

    bool mReevaluate = false;

    public void Reevaluate(bool result) {
        var cur = mMomentIT.Current;
        cur.mUser = false;
        cur.mExito = result;
        mReevaluate = true;
    }

    public IEnumerator Narrador() {
        mInteractiveType = InteractiveType.End;
        while ( mReevaluate || mMomentIT.MoveNext() ) {
            var cur = mMomentIT.Current;
            if (cur.mType == InteractiveType.Middle) mSegundaParte = true;
            InsertPanel(cur, mReevaluate);
            mReevaluate = false;
            if (!Skipping) yield return new WaitForSeconds(1.0f);
            if (cur.mLocal && cur.mUser) {
                yield return new WaitForSeconds(1.0f);
                mInteractiveType = cur.mType;
                break;
            }
        }
    }
}
