//
// The majority of the logic in model.js is from Elements.Playground v1.0.0
// https://github.com/hypar-io/Elements/blob/v1.0.0/Elements.Playground/wwwroot/js/model.js
//
// This model.js is therefore under their license:
// https://github.com/hypar-io/Elements/blob/v1.0.0/LICENSE.md

import * as THREE from 'https://cdn.skypack.dev/three@0.133.0/build/three.module.js'
import {OrbitControls} from 'https://cdn.skypack.dev/three@0.133.0/examples/jsm/controls/OrbitControls.js';
import {GLTFLoader} from 'https://cdn.skypack.dev/three@0.133.0/examples/jsm/loaders/GLTFLoader.js';


const scenes = {};

// https://threejs.org/docs/#examples/en/loaders/GLTFLoader
const loadModel = (key, uri) => {
  const loader = new GLTFLoader();
  console.log(`Loading model ${uri}`);
  loader.load(
    uri,
    function (gltf) {
      scenes[key].add(gltf.scene);
    },
    function (xhr) {
      console.log((xhr.loaded / xhr.total * 100) + "% loaded");
    },
    function (error) {
      console.log("An error happened");
      console.log(error);
      console.error(error);
    }
  );
}

const initialize3D = (key) => {
  const div = document.getElementById(`model-${key}`);

  console.log(`model-${key}`);
  //const div = document.querySelector(`#card-${key} .model`);

  scenes[key] = new THREE.Scene();

  if (!div) {
    console.error("Failed to initialise 3d. Couldn't find the element.");
    return;
  }

  const camera = new THREE.PerspectiveCamera(75, div.clientWidth / div.clientHeight, 0.1, 1000);
  const renderer = new THREE.WebGLRenderer({alpha: true, antialias: true});
  renderer.setSize(div.clientWidth, div.clientHeight);

  div.appendChild(renderer.domElement);

  const controls = new OrbitControls(camera, renderer.domElement);

  const directionalLight = new THREE.DirectionalLight(0xffffff, 1.0);
  directionalLight.position.set(0.5, 0.5, 0);
  scenes[key].add(directionalLight);

  const size = 100;
  const divisions = 20;
  const gridHelper = new THREE.GridHelper(size, divisions, "darkgray", "lightgray");
  scenes[key].add(gridHelper);

  const light = new THREE.HemisphereLight(0xffffbb, 0x080820, 1.0);
  scenes[key].add(light);

  camera.position.z = 5;
  camera.position.y = 10;

  controls.update();

  const animate = function () {
    requestAnimationFrame(animate);
    controls.update();
    renderer.render(scenes[key], camera);
  };

  window.addEventListener("resize", () => {
    camera.aspect = div.clientWidth / div.clientHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(div.clientWidth, div.clientHeight);
  }, false);

  animate();
}

export { initialize3D, loadModel };
