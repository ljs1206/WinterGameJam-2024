using System;
using System.Collections.Generic;
using LJS.Bullets;
using LJS.Entites;
using LJS.pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LJS.Enemys
{
    public enum BulletType{
        Normal = 0, Spread, Message, Circle,  End
    }

    public class EnemyAttack : EntityAttack
    {
        [Header("Spawn Setting")]
        [SerializeField] private PoolItemSO _NormalbulletName; // todo : fix to Pooling
        [SerializeField] private PoolItemSO _SpreadbulletName; // todo : fix to Pooling
        [SerializeField] private PoolItemSO _MessagebulletName; // todo : fix to Pooling
        [SerializeField] private PoolItemSO _CirclebulletName; // todo : fix to Pooling
        [SerializeField] private Transform _attackTrm;

        [Header("Attack Setting")]
        public Transform lookTarget;
        [SerializeField] private float _attackCoolTime; // todo : fix to Stat
        [SerializeField] private float _attackProbability; // todo : fix to Stat
        [SerializeField] private BulletType _bulletType;

        [Header("Bullet Setting")]
        [SerializeField] private List<BulletInfo> _damageTextList;
        [SerializeField] private List<BulletInfo> _healingTextList;

        private float _lastAttackTime = 0f;
        private AttackType _currentAttackType;
        private BulletInfo _currentBulletInfo;

        public override void Initialize(Entity entity)
        {
            base.Initialize(entity);
            lookTarget = FindObjectOfType<Player>().transform;
        }

        private void Update(){
            if(_lastAttackTime <= 0){
                CanAttack = true;
                _lastAttackTime = Time.time;
                return;
            } 
                
            if(Time.time - _lastAttackTime > _attackCoolTime){
                CanAttack = true;
                _lastAttackTime = Time.time;
            }
        }

        public override void ExcuteAttack()
        {
            CanAttack = false;
            RandomSelectAttackType();

            LJS.pool.IPoolable bullet = null;
            Bullet bulletCompo = null;
            switch (_bulletType){
                case BulletType.Normal:
                {
                    bullet = PoolManager.Instance.Pop(_NormalbulletName.poolName);
                }
                break;
                case BulletType.Spread:
                {
                    bullet = PoolManager.Instance.Pop(_SpreadbulletName.poolName);
                }
                break;
                case BulletType.Message:
                {
                    if(Vector3.Distance(_attackTrm.position, lookTarget.position) < 9f)
                    {
                        bullet = PoolManager.Instance.Pop(_NormalbulletName.poolName);
                        break;
                    }

                    bullet = PoolManager.Instance.Pop(_MessagebulletName.poolName);
                    bulletCompo = bullet.GetGameObject().GetComponent<Bullet>();
                    bullet.GetGameObject().transform.position = _attackTrm.position;
                    bulletCompo.SetBullet(_currentBulletInfo, _entity as Enemy, _attackTrm.position, true, default);
                    SpawnManager.Instance.AddSpawnedList(SpawnType.Bullet, bullet);
                    return;
                }
                case BulletType.Circle:
                {
                    bullet = PoolManager.Instance.Pop(_CirclebulletName.poolName);
                }
                break;
            }

            bulletCompo = bullet.GetGameObject().GetComponent<Bullet>();
            bulletCompo.SetBullet(_currentBulletInfo, _entity as Enemy, _attackTrm.position, true, default);
            bullet.GetGameObject().transform.position = _attackTrm.position;
            SpawnManager.Instance.AddSpawnedList(SpawnType.Bullet, bullet);
        }

        public void RandomSelectAttackType(){
            int randNum = Random.Range(0, 100);
            if(randNum <= _attackProbability){
                _currentAttackType = AttackType.Damage;
            }
            else{
                _currentAttackType = AttackType.Healing;
            }

            int randIndex = 0;
            switch(_currentAttackType){
                case AttackType.Damage:
                {
                    randIndex = Random.Range(0, _damageTextList.Count);
                    _currentBulletInfo = _damageTextList[randIndex];
                }
                break;
                case AttackType.Healing:
                {
                    randIndex = Random.Range(0, _healingTextList.Count);
                    _currentBulletInfo = _healingTextList[randIndex];
                }
                break;
            }
        }
    }
}
