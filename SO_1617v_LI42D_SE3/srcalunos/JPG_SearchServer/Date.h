#pragma once

#include <iostream>
#include <sstream>
#include <string>
 

using namespace std;

class Date {
private:
	bool valid;
	int year, month, day;
public:
	Date(string str) {
		cout << str << endl;
		istringstream input(str);
		char delim1, delim2;

		input >> year >> delim1 >> month >> delim2 >> day;

		valid = !input.bad();
	}

	bool isValid() { return valid;  }

	int compare(Date& d) {
		int diff = year - d.year;
		if (diff != 0) return diff;
		diff = month - d.month;
		if (diff != 0) return diff;
		return day - d.day;
	}

	bool isBetween(Date& di, Date& df) {
		return compare(di) >= 0 && compare(df) <= 0;
	}
};
