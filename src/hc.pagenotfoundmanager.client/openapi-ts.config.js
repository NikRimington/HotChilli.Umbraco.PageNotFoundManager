import { defineConfig } from '@hey-api/openapi-ts';

export default defineConfig({
	client: 'fetch',
	debug: true,
	input: 'http://localhost:9851/umbraco/swagger/hcs/swagger.json?urls.primaryName=Hot+Chilli+Api',
	output: {
		path: 'src/api',
		format: 'prettier',
		lint: 'eslint',
	},
	schemas: false,
	services: {
		asClass: true,
	},
	types: {
		enums: 'typescript',
	},
});