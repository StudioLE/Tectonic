// Based on Three.js samples
// Loading GlTF:
// https://github.com/mrdoob/three.js/blob/e22cb060cc91283d250e704f886528e1be593f45/examples/webgl_loader_gltf.html
// Lights:
// https://github.com/mrdoob/three.js/blob/e22cb060cc91283d250e704f886528e1be593f45/examples/webgl_lights_hemisphere.html

import * as THREE from 'three';

import {OrbitControls} from 'three/examples/jsm/controls/OrbitControls.js';
import {GLTFLoader} from 'three/examples/jsm/loaders/GLTFLoader.js';

// import { RGBELoader } from 'three/examples/jsm/loaders/RGBELoader.js';

class ModelViewer {

  private Container!: HTMLElement;
  private Scene: THREE.Scene = new THREE.Scene();
  private Bounds: THREE.Box3 = new THREE.Box3();
  private Center: THREE.Vector3 = new THREE.Vector3();
  private Near: number = 0.1;
  private Camera!: THREE.PerspectiveCamera;
  private Renderer!: THREE.WebGLRenderer;

  // async constructor: https://stackoverflow.com/a/50885340/247218
  constructor(id: string, uri: string) {
    return (async (): Promise<ModelViewer> => {

      this.InitContainer(id);
      await this.InitScene(uri);
      this.InitBounds();
      this.InitCamera();
      this.InitLights();
      this.InitRenderer();
      this.InitGround();

      this.Container.appendChild(this.Renderer.domElement);
      this.InitControls();
      window.addEventListener('resize', this.OnWindowResize);
      this.Render();

      return this;
    })() as unknown as ModelViewer;
  }

  InitContainer = (id: string): void => {
    const container = document.getElementById(`model-${id}`)
    if (container === null)
      throw new Error("Failed to initialise 3d. Couldn't find the element.");
    this.Container = container;
  };

  InitScene = async (uri: string): Promise<void> => {
    const loader = new GLTFLoader();
    const gltf = await loader.loadAsync(uri);

    gltf.scene.traverse(node => {
      node.castShadow = true;
      node.receiveShadow = true;
    });

    this.Scene.add(gltf.scene);
  };

  InitBounds = (): void => {
    this.Scene.updateMatrixWorld();
    this.Bounds.setFromObject(this.Scene);
    this.Center = new THREE.Vector3()
    this.Center.x = (this.Bounds.min.x + this.Bounds.max.x) * 0.5
    this.Center.y = (this.Bounds.min.y + this.Bounds.max.y) * 0.5
    this.Center.z = (this.Bounds.min.z + this.Bounds.max.z) * 0.5
    this.Near = this.Bounds.min.distanceTo(this.Bounds.max);
  };

  InitCamera = (): void => {
    this.Camera = new THREE.PerspectiveCamera( 45, this.Container.clientWidth / this.Container.clientHeight, 0.25, 20 );
    this.Camera.position.x = this.Near * 2.5;
    this.Camera.position.y = this.Near * 2.5;
    this.Camera.position.z = this.Near * 2.5;
  };

  InitLights = (): void => {
    const hemiLight = new THREE.HemisphereLight( '#ffffff', '#ffffff', 0.1 );
    this.Scene.add( hemiLight );

    // const hemiLightHelper = new THREE.HemisphereLightHelper( hemiLight, 1 );
    // this.Scene.add( hemiLightHelper );

    const dirLight = new THREE.DirectionalLight( '#ffffff', 0.75 );
    dirLight.position.set( -0.5, 1, 0.5 );
    dirLight.position.multiplyScalar( this.Near * 2 );
    dirLight.castShadow = true;
    this.Scene.add( dirLight );

    // const dirLightHelper = new THREE.DirectionalLightHelper( dirLight, 1 );
    // this.Scene.add( dirLightHelper );
  }

  InitGround = (): THREE.Mesh => {
    const plane = new THREE.PlaneGeometry( this.Near * 100, this.Near * 100 )
    const material = new THREE.MeshPhongMaterial( { color: '#1f2e54' } )
    const ground = new THREE.Mesh( plane,  material );
    ground.rotation.x = - Math.PI / 2;
    ground.position.y = this.Bounds.min.y;
    ground.receiveShadow = true;
    this.Scene.add( ground );

    this.Scene.background = new THREE.Color( '#1f2e54' );
    // this.Scene.fog = new THREE.Fog( '#1f2e54', this.Near * 2.5, this.Near * 10 );

    return ground;
  };

  InitRenderer = (): void => {
    this.Renderer = new THREE.WebGLRenderer( { alpha: true, antialias: true } );
    // renderer.setPixelRatio( window.devicePixelRatio );
    this.Renderer.setSize( this.Container.clientWidth, this.Container.clientHeight );
    // renderer.toneMapping = THREE.ACESFilmicToneMapping;
    // renderer.toneMappingExposure = 1;
    // renderer.outputEncoding = THREE.sRGBEncoding;
    this.Renderer.outputEncoding = THREE.sRGBEncoding;
    this.Renderer.shadowMap.enabled = true;
    this.Renderer.shadowMap.type = THREE.PCFSoftShadowMap;
  };

  InitControls = (): void => {
    const controls = new OrbitControls( this.Camera, this.Renderer.domElement );
    controls.addEventListener( 'change', this.Render ); // use if there is no animation loop
    controls.minDistance = 0;
    controls.maxDistance = this.Bounds.min.distanceTo(this.Bounds.max) * 4;
    controls.target = this.Center;
    controls.update();
  };

  OnWindowResize = () => {
    this.Camera.aspect = this.Container.clientWidth / this.Container.clientHeight;
    this.Camera.updateProjectionMatrix();
    this.Renderer.setSize( this.Container.clientWidth, this.Container.clientHeight );
    this.Render();
  };

  Render = () => {
    this.Renderer.render( this.Scene, this.Camera );
  };
}

export const init = (id: string, uri: string) => {
  return new ModelViewer(id, uri);
};
