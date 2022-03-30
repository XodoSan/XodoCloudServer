string = input()

numberArray = string.split(' ');
enteredDataArray = [1, 2, 3];
resultArray = [1, 2, 3];

for i in 0, 1, 2:
  if (float(numberArray[i]) < 1024):
    enteredDataArray[i] = round(float(numberArray[i]))
    resultArray[i] = str(bin(enteredDataArray[i])).count('1')
    
resultArray.sort()

print(', '.join(map(str, resultArray)))