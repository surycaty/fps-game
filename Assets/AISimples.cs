using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AISimples : MonoBehaviour
{

    public FOVEnemy _cabeca;
    NavMeshAgent _navMesh;
    Transform alvo;
    Vector3 posicInicialDaAI;
    Vector3 ultimaPosicConhecida;
    float timerProcura;

    public enum estadoDaAI
    {
        parado, seguindo, procurandoAlvoPerdido
    };
    
    public estadoDaAI _estadoAI = estadoDaAI.parado;

    public List<Transform> pathNodes = new List<Transform>();
    public float speedMoveEnemy = 2f;
    private Vector3 nextPosition;

    void Start()
    {
        _navMesh = GetComponent<NavMeshAgent>();
        alvo = null;
        ultimaPosicConhecida = Vector3.zero;
        _estadoAI = estadoDaAI.parado;
        posicInicialDaAI = transform.position;
        timerProcura = 0;

        nextPosition = pathNodes[0].position;
    }


    void Update()
    {
        if (_cabeca)
        {
            switch (_estadoAI)
            {
                case estadoDaAI.parado:

                    _navMesh.SetDestination(nextPosition);
                    if (_cabeca.inimigosVisiveis.Count > 0)
                    {
                        alvo = _cabeca.inimigosVisiveis[0];
                        ultimaPosicConhecida = alvo.position;
                        _estadoAI = estadoDaAI.seguindo;
                    } else
                    {
                        if (transform.position.x == pathNodes[0].position.x
                            && transform.position.z == pathNodes[0].position.z)
                        {
                            nextPosition = pathNodes[1].position;
                        }

                        if (transform.position.x == pathNodes[1].position.x
                            && transform.position.z == pathNodes[1].position.z)
                        {
                            nextPosition = pathNodes[0].position;
                        }

                        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speedMoveEnemy * Time.deltaTime);
                    }
                    break;
                case estadoDaAI.seguindo:
                    _navMesh.SetDestination(alvo.position);
                    if (!_cabeca.inimigosVisiveis.Contains(alvo))
                    {
                        ultimaPosicConhecida = alvo.position;
                        _estadoAI = estadoDaAI.procurandoAlvoPerdido;
                    }
                    break;
                case estadoDaAI.procurandoAlvoPerdido:
                    _navMesh.SetDestination(ultimaPosicConhecida);
                    timerProcura += Time.deltaTime;
                    if (timerProcura > 5)
                    {
                        timerProcura = 0;
                        _estadoAI = estadoDaAI.parado;
                        break;
                    }
                    if (_cabeca.inimigosVisiveis.Count > 0)
                    {
                        alvo = _cabeca.inimigosVisiveis[0];
                        ultimaPosicConhecida = alvo.position;
                        _estadoAI = estadoDaAI.seguindo;
                    }
                    break;
            }
        }
    }
}