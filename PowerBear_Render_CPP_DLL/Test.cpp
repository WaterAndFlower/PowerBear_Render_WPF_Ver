#include "pch.h"
#include "Test.h"

int TestDLL(const unsigned long long a, const unsigned long long b) {
	return a + b;
	//return "和C++的DLL链接成功了，执行加法运算：" + std::to_string(a + b);
}

char TestString() {
	return 'A';
}