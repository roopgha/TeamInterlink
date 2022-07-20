using System.Collections;

public interface IAction
{
	public enum Direction { Left, Right, Up, Guard, LeftUp, LeftDown, RightUp, RightDown, None };
	public enum Skill { Attack, ChargedAttack, JumpAttack, LethalMove, None };

	public void Attack();

	public void ChargedAttack();

	public void JumpAttack();

	public void LethalMove();
}