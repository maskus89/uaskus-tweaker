import { useEffect, useRef } from "react";

const GRID_SPACING = 24;
const REPEL_RADIUS = 240;
const MAX_OFFSET = 14;
const ACTIVE_EASE = 0.16;
const RETURN_EASE = 0.12;
const SETTLE_EPSILON = 0.02;
const BASE_SIZE = 1.85;
const BOOST_SIZE = 1.05;
const MAX_DEVICE_PIXEL_RATIO = 1.5;
const IDLE_AMPLITUDE_MIN = 0.18;
const IDLE_AMPLITUDE_MAX = 1.05;
const IDLE_SPEED_MIN = 0.18;
const IDLE_SPEED_MAX = 0.44;

type GridBuffers = {
  baseX: Float32Array;
  baseY: Float32Array;
  offsetX: Float32Array;
  offsetY: Float32Array;
  idleAmplitude: Float32Array;
  idleSpeed: Float32Array;
  idlePhaseX: Float32Array;
  idlePhaseY: Float32Array;
  count: number;
  width: number;
  height: number;
};

function fract(value: number) {
  return value - Math.floor(value);
}

function pseudoRandom(seed: number) {
  return fract(Math.sin(seed * 12.9898) * 43758.5453123);
}

function createGrid(width: number, height: number): GridBuffers {
  const cols = Math.ceil(width / GRID_SPACING) + 2;
  const rows = Math.ceil(height / GRID_SPACING) + 2;
  const count = cols * rows;
  const baseX = new Float32Array(count);
  const baseY = new Float32Array(count);
  const offsetX = new Float32Array(count);
  const offsetY = new Float32Array(count);
  const idleAmplitude = new Float32Array(count);
  const idleSpeed = new Float32Array(count);
  const idlePhaseX = new Float32Array(count);
  const idlePhaseY = new Float32Array(count);
  const start = GRID_SPACING / 2;

  let index = 0;

  for (let row = 0; row < rows; row += 1) {
    for (let col = 0; col < cols; col += 1) {
      const seed = index + 1;
      const randomA = pseudoRandom(seed);
      const randomB = pseudoRandom(seed * 1.7);
      const randomC = pseudoRandom(seed * 2.3);
      const randomD = pseudoRandom(seed * 3.1);

      baseX[index] = start + col * GRID_SPACING;
      baseY[index] = start + row * GRID_SPACING;
      idleAmplitude[index] =
        IDLE_AMPLITUDE_MIN +
        randomA * (IDLE_AMPLITUDE_MAX - IDLE_AMPLITUDE_MIN);
      idleSpeed[index] =
        IDLE_SPEED_MIN + randomB * (IDLE_SPEED_MAX - IDLE_SPEED_MIN);
      idlePhaseX[index] = randomC * Math.PI * 2;
      idlePhaseY[index] = randomD * Math.PI * 2;
      index += 1;
    }
  }

  return {
    baseX,
    baseY,
    offsetX,
    offsetY,
    idleAmplitude,
    idleSpeed,
    idlePhaseX,
    idlePhaseY,
    count,
    width,
    height
  };
}

export default function InteractiveDotGrid() {
  const canvasRef = useRef<HTMLCanvasElement | null>(null);

  useEffect(() => {
    const canvas = canvasRef.current;

    if (!canvas) {
      return;
    }

    const context = canvas.getContext("2d", { alpha: true });

    if (!context) {
      return;
    }

    const mediaQuery = window.matchMedia("(prefers-reduced-motion: reduce)");
    const pointer = { x: 0, y: 0, active: false };
    let reduceMotion = mediaQuery.matches;
    let isVisible = !document.hidden;
    let frameId = 0;
    let devicePixelRatio = 1;
    let grid = createGrid(window.innerWidth, window.innerHeight);

    const draw = (timestamp = performance.now()) => {
      frameId = 0;

      context.clearRect(0, 0, grid.width, grid.height);
      context.fillStyle = "#b8d2ff";

      const radiusSquared = REPEL_RADIUS * REPEL_RADIUS;
      const time = timestamp * 0.001;
      let keepAnimating = false;

      for (let index = 0; index < grid.count; index += 1) {
        let targetX = 0;
        let targetY = 0;
        let influence = 0;
        let idleX = 0;
        let idleY = 0;

        if (!reduceMotion) {
          const idleSpeed = grid.idleSpeed[index];
          const idleAmplitude = grid.idleAmplitude[index];

          idleX =
            Math.sin(time * idleSpeed + grid.idlePhaseX[index]) * idleAmplitude;
          idleY =
            Math.cos(
              time * (idleSpeed * 0.82) + grid.idlePhaseY[index]
            ) *
            idleAmplitude *
            0.8;
        }

        if (!reduceMotion && pointer.active) {
          const deltaX = grid.baseX[index] + idleX - pointer.x;
          const deltaY = grid.baseY[index] + idleY - pointer.y;
          const distanceSquared = deltaX * deltaX + deltaY * deltaY;

          if (distanceSquared < radiusSquared) {
            const distance = Math.max(Math.sqrt(distanceSquared), 0.001);
            influence = 1 - distance / REPEL_RADIUS;

            const force = influence * influence * MAX_OFFSET;

            targetX = (deltaX / distance) * force;
            targetY = (deltaY / distance) * force;
          }
        }

        const easing = pointer.active ? ACTIVE_EASE : RETURN_EASE;

        grid.offsetX[index] += (targetX - grid.offsetX[index]) * easing;
        grid.offsetY[index] += (targetY - grid.offsetY[index]) * easing;

        if (
          Math.abs(grid.offsetX[index]) > SETTLE_EPSILON ||
          Math.abs(grid.offsetY[index]) > SETTLE_EPSILON
        ) {
          keepAnimating = true;
        }

        const x = grid.baseX[index] + idleX + grid.offsetX[index];
        const y = grid.baseY[index] + idleY + grid.offsetY[index];
        const size = BASE_SIZE + influence * BOOST_SIZE;

        context.globalAlpha = 0.24 + influence * 0.42;
        context.fillRect(x - size / 2, y - size / 2, size, size);
      }

      context.globalAlpha = 1;

      if ((!reduceMotion && isVisible) || pointer.active || keepAnimating) {
        frameId = window.requestAnimationFrame(draw);
      }
    };

    const requestDraw = () => {
      if (frameId === 0) {
        frameId = window.requestAnimationFrame(draw);
      }
    };

    const resizeCanvas = () => {
      const width = window.innerWidth;
      const height = window.innerHeight;

      devicePixelRatio = Math.min(
        window.devicePixelRatio || 1,
        MAX_DEVICE_PIXEL_RATIO
      );

      canvas.width = Math.round(width * devicePixelRatio);
      canvas.height = Math.round(height * devicePixelRatio);
      canvas.style.width = `${width}px`;
      canvas.style.height = `${height}px`;

      context.setTransform(devicePixelRatio, 0, 0, devicePixelRatio, 0, 0);

      grid = createGrid(width, height);
    };

    const handlePointerMove = (event: PointerEvent) => {
      if (reduceMotion || event.pointerType === "touch") {
        return;
      }

      pointer.x = event.clientX;
      pointer.y = event.clientY;
      pointer.active = true;

      requestDraw();
    };

    const handlePointerLeave = () => {
      if (!pointer.active) {
        return;
      }

      pointer.active = false;
      requestDraw();
    };

    const handlePointerOut = (event: PointerEvent) => {
      if (event.relatedTarget === null) {
        handlePointerLeave();
      }
    };

    const handleResize = () => {
      resizeCanvas();
      requestDraw();
    };

    const handleVisibilityChange = () => {
      if (document.hidden) {
        isVisible = false;
        pointer.active = false;

        if (frameId !== 0) {
          window.cancelAnimationFrame(frameId);
          frameId = 0;
        }

        return;
      }

      isVisible = true;
      requestDraw();
    };

    const handleReducedMotionChange = () => {
      reduceMotion = mediaQuery.matches;
      pointer.active = false;
      resizeCanvas();
      requestDraw();
    };

    resizeCanvas();
    requestDraw();

    window.addEventListener("resize", handleResize);
    window.addEventListener("pointermove", handlePointerMove, { passive: true });
    window.addEventListener("pointerout", handlePointerOut);
    window.addEventListener("blur", handlePointerLeave);
    document.addEventListener("visibilitychange", handleVisibilityChange);

    if (typeof mediaQuery.addEventListener === "function") {
      mediaQuery.addEventListener("change", handleReducedMotionChange);
    } else {
      mediaQuery.addListener(handleReducedMotionChange);
    }

    return () => {
      if (frameId !== 0) {
        window.cancelAnimationFrame(frameId);
      }

      window.removeEventListener("resize", handleResize);
      window.removeEventListener("pointermove", handlePointerMove);
      window.removeEventListener("pointerout", handlePointerOut);
      window.removeEventListener("blur", handlePointerLeave);
      document.removeEventListener("visibilitychange", handleVisibilityChange);

      if (typeof mediaQuery.removeEventListener === "function") {
        mediaQuery.removeEventListener("change", handleReducedMotionChange);
      } else {
        mediaQuery.removeListener(handleReducedMotionChange);
      }
    };
  }, []);

  return <canvas aria-hidden="true" className="site-grid" ref={canvasRef} />;
}
