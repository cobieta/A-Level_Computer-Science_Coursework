using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMathQuestion : MonoBehaviour {

	private int numberOfMoves, startNumber, targetNumber;
	private List<int[]> answerSequence; 
	
	//This function is called by the options menu class to get the number of
	//moves, the target number and the starting number of the question this 
	//class generates.
	public int[] GetMathsQuestion () {
		GenerateQuestion();
		int[] results = new int[3] {numberOfMoves, startNumber, targetNumber};
		return results;
	}

	//This function is called by the options class menu to get the answer 
	//sequence of the question generated. 
	public List<int[]> GetAnswerSequence () {
		return answerSequence;
	}

	//This procedure performs all the required steps to generate a maths 
	//question such as generating a target number and the number of moves 
	//the question will take to solve. It generates each move and adds 
	//them to a sequence to be passed to the options class.
	void GenerateQuestion () {
		answerSequence = new List<int[]>();
		targetNumber = Random.Range(PersistentGameData.rangeMin, PersistentGameData.rangeMax + 1);
		numberOfMoves = Random.Range(2, PersistentGameData.maxMoves +1);
		startNumber = targetNumber;
		for (int moveNumber = 0; moveNumber < numberOfMoves; moveNumber++) {
			int[] moveGenerated = GenerateMove(startNumber);
			answerSequence.Add(moveGenerated);	
		}
	}

	//This function generates a move by generating an operator and an integer 
	//that the operation will be performed on. The start number is modified by 
	//performing the opposite operator on it. When the operator chosen is 
	//multiplication, the operator number can only be a factor of the current
	//target to keep all the sums using integer values. 
	int[] GenerateMove (int currentTarget) {
		int operatorsNumber = Random.Range(1, currentTarget+1);
		int operation = Random.Range(0, PersistentGameData.operatorCodesArray.Count);
		char thisOperator = PersistentGameData.operatorCodesArray[operation];
		switch (thisOperator) {
			case '+':
				startNumber -= operatorsNumber;
				break;
			case '-':
				startNumber += operatorsNumber;
				break;
			case '÷': 
				startNumber *= operatorsNumber; 
				break;
			default:
				if ((operatorsNumber % currentTarget) != 0) {
					operatorsNumber = FindIntegerDivisor(currentTarget);
				}
				startNumber /= operatorsNumber;
				break;
		}
		int operatorUnicode = thisOperator;
		int[] move = new int[] {operatorUnicode, operatorsNumber};
		return move;
	}

	//Finds the prime factors of a number and multiplies a combination of them to find
	//a integer number that goes into the target. 
	int FindIntegerDivisor (int target) {
		List<int> primeList = new List<int>();
		//Find the prime factors that are 2.
		while ((target % 2) == 0) {
			primeList.Add(2);
			target /= 2;
		}
		//target is now an odd number
		for (int i = 3; i <= Mathf.Sqrt(target); i += 2) {
			//while i is a factor of target, add i to prime list and divide by target.
			while ((target % i) == 0) {
				primeList.Add(i);
				target /= i;
			}
		}
		//If target is a prime number greater than 2.
		if (target > 2) {
			primeList.Add(target);
		}
		//From the list of prime factors pick randomly between 1 and the number of factors
		//and multiply them together.
		int result = 1;
		int numberOfFactorsMultiplied = Random.Range(1, primeList.Count);
		for (int j = 1; j <= numberOfFactorsMultiplied; j++) {
			int randomListPos = Random.Range(0, primeList.Count);
			result *= primeList[randomListPos];
		}
		//Return the result
		return result;
	}
}
