import { readFileSync, writeFileSync } from 'node:fs';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const repositoryRoot=resolve(dirname(fileURLToPath(import.meta.url)),'..');
const props=readFileSync(resolve(repositoryRoot,'Directory.Build.props'),'utf8');
const version=props.match(/<Version>([^<]+)<\/Version>/)?.[1]?.trim();
if(!version)throw new Error('Project version was not found.');
const suppliedCommit=process.env.BUILD_COMMIT?.trim()??'';
if(suppliedCommit&&!/^[a-f0-9]{7,40}$/i.test(suppliedCommit))throw new Error('BUILD_COMMIT is invalid.');
const commit=suppliedCommit.slice(0,7);
const contents=`export const BUILD_INFO = {
  version: '${version}',
  commit: '${commit}'
} as const;\n`;
writeFileSync(resolve(repositoryRoot,'src/frontend/ruta-cero-web/src/app/core/build-info.ts'),contents,'utf8');
