using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState {

	public abstract WaitForSeconds Enter(Enemy enemy);
	public abstract void Execute(Enemy enemy);
	public abstract WaitForSeconds Exit(Enemy enemy);

}

namespace Bastion.EnemyAIStates {

	public class GoToSupply : EnemyState {
		
		public override WaitForSeconds Enter(Enemy enemy) {
				
			enemy.Agent.SetDestination( GameManager.Instance.SupplyBox.gameObject.transform.position );

			return new WaitForSeconds(0);
		}
		
		
		
		public override void Execute(Enemy enemy) {

			Vector3 curPosition = enemy.transform.position;
			
			float distanceToPlayer = Vector3.Distance(curPosition, GameManager.Instance.PlayerPosition);
			float distanceToSupply = Vector3.Distance(curPosition, GameManager.Instance.SupplyBox.transform.position);
			
			if (distanceToPlayer < enemy.DetectDistance) // 플레이어가 감지범위 안으로 들어왔을 때
				enemy.StartChangeState(EnemyStates.ChasePlayer);
				// 플레이어를 쫒기 시작한다

			if (distanceToSupply < enemy.Agent.stoppingDistance) // SupplyBox가 공격범위 안으로 들어왔을 때
				enemy.StartChangeState(EnemyStates.AttackSupply);
				// Supply를 공격하기 시작한다
			
		}
		
		
		
		public override WaitForSeconds Exit(Enemy enemy) {
			
			return new WaitForSeconds(0);
		}
		
	}



	public class ChasePlayer : EnemyState {
		
		public override WaitForSeconds Enter(Enemy enemy) {
			
			return new WaitForSeconds(0);
		}
		
		public override void Execute(Enemy enemy) {

			enemy.Agent.SetDestination(GameManager.Instance.PlayerPosition);

			float distanceToPlayer = Vector3.Distance(enemy.transform.position, GameManager.Instance.PlayerPosition);
			
			if (distanceToPlayer > enemy.DetectDistance) { // 플레이어가 감지 범위를 벗어나면
				enemy.StartChangeState(EnemyStates.GoToSupply);
				// 다시 Supply를 쫒는다
			} 

			if (distanceToPlayer < enemy.AttackDistance) { // 플레이어가 공격범위 안으로 들어오면
				enemy.StartChangeState(EnemyStates.AttackPlayer);
				Debug.DrawLine(enemy.transform.position, GameManager.Instance.PlayerPosition);
				// 공격한다
			}
		}
		
		public override WaitForSeconds Exit(Enemy enemy) {
			
			return new WaitForSeconds(0);
		}
		
	}



	public class AttackPlayer : EnemyState {
		
		public override WaitForSeconds Enter(Enemy enemy) {
			
			enemy.StartCoroutine(enemy.Attack());
				// 상태 진입 시 공격 함수를 호출한다
			
			return new WaitForSeconds(0);
		}
		
		public override void Execute(Enemy enemy) {

			if (!enemy.IsInAttack) { // 공격이 끝나면 이전 상태로 돌아간다
				enemy.RevertToPreviousState();
			}
		}
		
		public override WaitForSeconds Exit(Enemy enemy) {
			
			return new WaitForSeconds(0);
		}
		
		
	}


	
	public class AttackSupply : EnemyState {
		
		public override WaitForSeconds Enter(Enemy enemy) {
			
			return new WaitForSeconds(0);
		}
		
		public override void Execute(Enemy enemy) {

			float distanceToSupply = Vector3.Distance(enemy.transform.position, GameManager.Instance.SupplyBox.transform.position);

			if (!enemy.IsInAttack && distanceToSupply < enemy.AttackDistance) // SupplyBox가 범위 안에 있을때 계속해서 공격함
				enemy.StartCoroutine(enemy.Attack());
		}
		
		public override WaitForSeconds Exit(Enemy enemy) {
			
			return new WaitForSeconds(0);
		}
		
	}

}
