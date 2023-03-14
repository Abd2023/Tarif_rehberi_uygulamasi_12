#include <stdio.h>

int main() {
    int arr1[] = {1, 2, 3, 4};
    int arr2[] = {2, 4, 6, 8, 10};
    int merged[9];
    int i, j, k, size1, size2, mergedSize = 0, duplicate;

    size1 = sizeof(arr1)/sizeof(arr1[0]);
    size2 = sizeof(arr2)/sizeof(arr2[0]);

    for (i = 0; i < size1; i++) {
        merged[mergedSize++] = arr1[i];
    }

    for (i = 0; i < size2; i++) {
        duplicate = 0;
        for (j = 0; j < mergedSize; j++) {
            if (arr2[i] == merged[j]) {
                duplicate = 1;
                break;
            }
        }
        if (!duplicate) {
            merged[mergedSize++] = arr2[i];
        }
    }

    printf("Merged array without duplicates: ");
    for (k = 0; k < mergedSize; k++) {
        printf("%d ", merged[k]);
    }

    return 0;
}
