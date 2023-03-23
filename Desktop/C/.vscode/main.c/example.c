#include <stdio.h>

int main() {
    int numRows, numCols;
    printf("Enter the number of rows: ");
    scanf("%d", &numRows);
    printf("Enter the number of columns: ");
    scanf("%d", &numCols);

    int myArray[numRows][numCols];
    int i, j, sum;

    // Read in the values for the array
    printf("Enter the values for the array:\n");
    for (i = 0; i < numRows; i++) {
        for (j = 0; j < numCols; j++) {
            scanf("%d", &myArray[i][j]);
        }
    }

    // Calculate the sum of each column
    for (j = 0; j < numCols; j++) {
        sum = 0;
        for (i = 0; i < numRows; i++) {
            sum += myArray[i][j];
        }
        printf("Sum of column %d: %d\n", j+1, sum);
    }

    return 0;
    
}
