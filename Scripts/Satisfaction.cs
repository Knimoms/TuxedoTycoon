using Godot;
using System;

public class Satisfaction : Node
{
  
	int indvCustomerSatisfaction(bool hasSpace, bool isRecipeCorrect, double waitingTime, int queueTime){ //individual customer satisfaction level
		int satisfaction = 50;
	
    if (!hasSpace) {
        satisfaction -= 25;  // Decrease by 25 if no space is available
    }
    
    if (!isRecipeCorrect) {
        satisfaction -= 25;  // Decrease by 25 if the recipe is wrong
        }

        if (hasSpace) {
        satisfaction += 25;  // Increase by 25 if the customer can sit
    }
    
    if (isRecipeCorrect) {
        satisfaction += 25;  // Increase by 25 if the recipe is correct
    }

     // Calculate satisfaction deduction based on queue time
     satisfaction -= queueTime;
        //1 sec -1 satisfaction
        // Ensure the minimum satisfaction is 0
        satisfaction = Math.Max(0, satisfaction);

    return satisfaction;

	}
	double averageSatisfaction( int satisfactionScores, int numCustomers){
		double sum = 0.0;
    
    for (int i = 0; i < numCustomers; i++) {
        sum += satisfactionScores[i];
		//array for customers and individual scores
		 }
     return sum / numCustomers;
}
int main() {
    
  for (int i = 0; i < numCustomers; i++)
        {
            int satisfaction = MeasureCustomerSatisfaction(hasSpace[i], isRecipeCorrect[i], queueTimes[i]);
            satisfactionSum += satisfaction;
        }

        // Calculate the average customer satisfaction
        double averageSatisfaction = (double)satisfactionSum / numCustomers;
        Console.WriteLine("Average customer satisfaction: " + averageSatisfaction);
    }
 }


	

