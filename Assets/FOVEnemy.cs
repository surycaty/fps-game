using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CollisionType
{
    RayCast, OverlapSphere
}

public enum CheckType
{
    _10PorSegundo, _20PorSegundo, OTempoTodo
}

public class FOVEnemy : MonoBehaviour
{
    [Header("General")]
    public Transform headCamera;
    public CollisionType collisionType = CollisionType.RayCast;
    public CheckType checkType = CheckType.OTempoTodo;
    [Range(1, 50)]
    public float distanciaDeVisao = 10;

    [Header("OverlapSphere")]
    public LayerMask layersDosInimigos = 2;
    public bool desenharEsfera = true;

    [Header("Raycast")]
    public string tagDosInimigos = "Respawn";
    [Range(2, 180)]
    public float raiosExtraPorCamada = 20;
    [Range(5, 180)]
    public float anguloDeVisao = 120;
    [Range(1, 10)]
    public int numeroDeCamadas = 3;
    [Range(0.02f, 0.15f)]
    public float distanciaEntreCamadas = 0.1f;
    //
    [Space(10)]
    public List<Transform> inimigosVisiveis = new List<Transform>();
    List<Transform> listaTemporariaDeColisoes = new List<Transform>();
    LayerMask layerObstaculos;
    float timerChecagem = 0;

    private void Start()
    {
        timerChecagem = 0;
        if (!headCamera)
        {
            headCamera = transform;
        }
        // o operador ~ inverte o estado dos bits (0 passa a ser 1, e vice versa)
        layerObstaculos = ~layersDosInimigos;
    }

    void Update()
    {
        if (checkType == CheckType._10PorSegundo)
        {
            timerChecagem += Time.deltaTime;
            if (timerChecagem >= 0.1f)
            {
                timerChecagem = 0;
                ChecarInimigos();
            }
        }
        if (checkType == CheckType._20PorSegundo)
        {
            timerChecagem += Time.deltaTime;
            if (timerChecagem >= 0.05f)
            {
                timerChecagem = 0;
                ChecarInimigos();
            }
        }
        if (checkType == CheckType.OTempoTodo)
        {
            ChecarInimigos();
        }
    }

    private void ChecarInimigos()
    {
        if (collisionType == CollisionType.RayCast)
        {
            float limiteCamadas = numeroDeCamadas * 0.5f;
            for (int x = 0; x <= raiosExtraPorCamada; x++)
            {
                for (float y = -limiteCamadas + 0.5f; y <= limiteCamadas; y++)
                {
                    float angleToRay = x * (anguloDeVisao / raiosExtraPorCamada) + ((180.0f - anguloDeVisao) * 0.5f);
                    Vector3 directionMultipl = (-headCamera.right) + (headCamera.up * y * distanciaEntreCamadas);
                    Vector3 rayDirection = Quaternion.AngleAxis(angleToRay, headCamera.up) * directionMultipl;
                    //
                    RaycastHit hitRaycast;
                    if (Physics.Raycast(headCamera.position, rayDirection, out hitRaycast, distanciaDeVisao))
                    {
                        if (!hitRaycast.transform.IsChildOf(transform.root) && !hitRaycast.collider.isTrigger)
                        {
                            if (hitRaycast.collider.gameObject.CompareTag(tagDosInimigos))
                            {
                                Debug.DrawLine(headCamera.position, hitRaycast.point, Color.red);
                                //
                                if (!listaTemporariaDeColisoes.Contains(hitRaycast.transform))
                                {
                                    listaTemporariaDeColisoes.Add(hitRaycast.transform);
                                }
                                if (!inimigosVisiveis.Contains(hitRaycast.transform))
                                {
                                    inimigosVisiveis.Add(hitRaycast.transform);
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.DrawRay(headCamera.position, rayDirection * distanciaDeVisao, Color.green);
                    }
                }
            }
        }
        if (collisionType == CollisionType.OverlapSphere)
        {
            Collider[] alvosNoRaioDeAlcance = Physics.OverlapSphere(headCamera.position, distanciaDeVisao, layersDosInimigos);
            foreach (Collider targetCollider in alvosNoRaioDeAlcance)
            {
                Transform alvo = targetCollider.transform;
                Vector3 direcaoDoAlvo = (alvo.position - headCamera.position).normalized;
                if (Vector3.Angle(headCamera.forward, direcaoDoAlvo) < (anguloDeVisao / 2.0f))
                {
                    float distanciaDoAlvo = Vector3.Distance(transform.position, alvo.position);
                    if (!Physics.Raycast(headCamera.position, direcaoDoAlvo, distanciaDoAlvo, layerObstaculos))
                    {
                        if (!alvo.transform.IsChildOf(headCamera.root))
                        {
                            if (!listaTemporariaDeColisoes.Contains(alvo))
                            {
                                listaTemporariaDeColisoes.Add(alvo);
                            }
                            if (!inimigosVisiveis.Contains(alvo))
                            {
                                inimigosVisiveis.Add(alvo);
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < inimigosVisiveis.Count; x++)
            {
                Debug.DrawLine(headCamera.position, inimigosVisiveis[x].position, Color.blue);
            }
        }
        //remove da lista inimigos que não estão visiveis
        for (int x = 0; x < inimigosVisiveis.Count; x++)
        {
            if (!listaTemporariaDeColisoes.Contains(inimigosVisiveis[x]))
            {
                inimigosVisiveis.Remove(inimigosVisiveis[x]);
            }
        }
        listaTemporariaDeColisoes.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (collisionType == CollisionType.OverlapSphere)
        {
            if (desenharEsfera)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(headCamera.position, distanciaDeVisao);
            }
            Gizmos.color = Color.green;
            float angleToRay1 = (180.0f - anguloDeVisao) * 0.5f;
            float angleToRay2 = anguloDeVisao + (180.0f - anguloDeVisao) * 0.5f;
            Vector3 rayDirection1 = Quaternion.AngleAxis(angleToRay1, headCamera.up) * (-transform.right);
            Vector3 rayDirection2 = Quaternion.AngleAxis(angleToRay2, headCamera.up) * (-transform.right);
            Gizmos.DrawRay(headCamera.position, rayDirection1 * distanciaDeVisao);
            Gizmos.DrawRay(headCamera.position, rayDirection2 * distanciaDeVisao);
            //
            UnityEditor.Handles.color = Color.green;
            float angle = Vector3.Angle(transform.forward, rayDirection1);
            Vector3 pos = headCamera.position + (headCamera.forward * distanciaDeVisao * Mathf.Cos(angle * Mathf.Deg2Rad));
            UnityEditor.Handles.DrawWireDisc(pos, headCamera.transform.forward, distanciaDeVisao * Mathf.Sin(angle * Mathf.Deg2Rad));
        }
    }
#endif
}