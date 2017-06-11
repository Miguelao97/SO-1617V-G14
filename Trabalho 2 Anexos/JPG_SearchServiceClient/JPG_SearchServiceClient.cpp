#include <Windows.h>
#include <stdio.h>
#include "../Include/JPG_SearchService.h"
#include "../Include/PrintUtils.h"

INT main(INT argc, PCHAR argv[]) {

	if (argc < 4) {
		printf("Use: %s <service name> <repository path> <tag exif>\n", argv[0]);
		return 0;
	}
	PCHAR serviceName = argv[1];
	PCHAR repository = argv[2];
	PCHAR filter = argv[3];

	// Open Service
	PJPG_SEARCH_SERVICE service = JPG_SearchServiceOpen(serviceName);
	if (service == NULL) return 0;

	// Do request
	PCHAR answer = JPG_SearchServiceProcessRepository(service, repository, filter);
	// Show answer
	printf("%s\n", answer);

	// Release answer
	JPG_SearchReleaseAnswer(answer);

	// Close Service
	JPG_SearchServiceClose(service);

	PRESS_TO_FINISH("");
	return 0;
}