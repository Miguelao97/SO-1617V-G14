#pragma once
#include <iostream>
#include <sstream>
#include <string>


using namespace std;

class Rational {
private:
	bool valid;
	int numerator, denominator;
	float floatValue;
public:
	Rational(int r[2]) {
		numerator = r[0];
		denominator = r[1];
		if (denominator == 0) valid = false;
		else {
			valid = true;
			floatValue = ((int)(((float)numerator / denominator) * 10)) / 10.0F;
		}
	}

	Rational(string str) {
		istringstream input(str);
		 
		input >> floatValue;
		numerator = (int) (floatValue * 10);
		denominator = 10;
		valid = !input.bad();
		  
	}

	bool isValid() { return valid; }

	int compare(Rational& d) {
		return (int) (floatValue - d.floatValue);
	}

	bool isBetween(Rational& di, Rational& df) {
		return compare(di) >= 0 && compare(df) <= 0;
	}

	void show(char * prefix) {
		printf("%s=%f\n", prefix, floatValue);
	}
};

